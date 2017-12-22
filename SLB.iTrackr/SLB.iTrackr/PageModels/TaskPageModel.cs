using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreshMvvm;
using PropertyChanged;
using SLB.iTrackr.Models;
using SLB.iTrackr.Utils;
using Xamarin.Forms;

namespace SLB.iTrackr.PageModels
{
    [ImplementPropertyChanged]
    public class TaskPageModel : FreshBasePageModel
    {
        public string Title { get; set; }
        public List<Ticket> Tickets { get; set; }
        public bool IsBusy { get; set; }
        public bool IsNotBusy { get { return !IsBusy; } set { IsNotBusy = value; } }
        
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

        private TaskPageParam _pageParam;
        private Ticket _selectedTicket;

        public TaskPageModel()
        {                
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            _pageParam = (TaskPageParam)initData;
            Title = _pageParam.Title;
            IsBusy = false;
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            MessagingCenter.Subscribe<SPService, string>(this, "SP_REQUEST_STATUS", (s, args) =>
            {
                CoreMethods.DisplayAlert("Notification", args, "OK");
            });

            GetTickets();
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            MessagingCenter.Unsubscribe<SPService, string>(this, "SP_REQUEST_STATUS");
        }

        private async void NavigateToDetail(Ticket t)
        {
            await CoreMethods.PushPageModel<TaskDetailPageModel>(t);
        }

        private async void GetTickets()
        {
            var sp = new SPService(_pageParam.Credential, _pageParam.URL);

            IsBusy = true;
            Tickets = await sp.GetTicketByStatus(_pageParam.Title, _pageParam.ClientsId);
            IsBusy = false;
        }
    }
    
}
