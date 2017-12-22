using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;
using System.Windows.Input;
using Xamarin.Forms;
using SLB.iTrackr.Models;
using Plugin.Settings;
using SLB.iTrackr.Utils;

namespace SLB.iTrackr.PageModels
{
    [ImplementPropertyChanged]
    public class SearchPageModel : FreshBasePageModel
    {
        public ICommand SearchCMD { get; set; }
        public bool IsBusy { get; set; }
        public bool IsNotBusy { get { return !IsBusy; } set { IsNotBusy = value; } }
        public List<Ticket> Tickets { get; set; }

        private Ticket _selectedTicket;
        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set
            {
                _selectedTicket = value;
                if (_selectedTicket != null)
                    NavigateToDetail(_selectedTicket);
            }
        }

        private Credential _credential;
        private string _url;

        public SearchPageModel()
        {

        }

        public override void Init(object initData)
        {
            base.Init(initData);

            _credential = DependencyService.Get<ICredentialService>().GetCredential();
            _url = CrossSettings.Current.GetValueOrDefault<string>("URL");
            IsBusy = false;

            if(_credential != null && _url != null)
            {
                SearchCMD = new Command<string>(SearchTicket);
            }
            else
            {
                CoreMethods.DisplayAlert("Error", "Configuration incomplete!", "OK");
            }
           
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            MessagingCenter.Subscribe<SPService, string>(this, "SP_REQUEST_STATUS", (s, args) =>
            {
                CoreMethods.DisplayAlert("Notification", args, "OK");
            });

            SelectedTicket = null;
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            MessagingCenter.Unsubscribe<SPService, string>(this, "SP_REQUEST_STATUS");
        }

        private async void SearchTicket(string key)
        {
            var sp = new SPService(_credential, _url);

            if (Tickets != null)
                Tickets.Clear();

            IsBusy = true;
            Tickets = await sp.GetTicketByParam(key);
            IsBusy = false;
        }

        private async void NavigateToDetail(Ticket t)
        {
            await CoreMethods.PushPageModel<TaskDetailPageModel>(t);
        }
    }
}
