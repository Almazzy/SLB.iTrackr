using System;
using FreshMvvm;
using Xamarin.Forms;
using SLB.iTrackr.Utils;
using SLB.iTrackr.Models;
using SLB.iTrackr.Pages;
using SLB.iTrackr.Navigation;
using System.Windows.Input;
using PropertyChanged;
using System.Collections.Generic;
using Plugin.Settings;
using System.Linq;

namespace SLB.iTrackr.PageModels
{
    [ImplementPropertyChanged]
    public class HomePageModel : FreshBasePageModel
    {
        private Credential _credential;
        private string _url;
        private List<string> _clients;

        public ICommand SearchCommand { get; set; }
        public ICommand MenuTapped { get; private set; }
        public ImageSource Task1ChartImage { get; private set; }
        public ImageSource Task2ChartImage { get; private set; }
        public ImageSource Task3ChartImage { get; private set; }
        public ImageSource Task4ChartImage { get; private set; }
        public ImageSource Task5ChartImage { get; private set; }
        public ImageSource Task6ChartImage { get; private set; }
        public double DaysToComplete { get; private set; }
        public bool IsBusy { get; set; }
        public bool IsNotBusy { get { return !IsBusy; } set { IsNotBusy = value; } }

        public HomePageModel()
        {
            
        }        

        public override void Init(object initData)
        {
            base.Init(initData);            
            IsBusy = false;
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            _credential = DependencyService.Get<ICredentialService>().GetCredential();
            _url = CrossSettings.Current.GetValueOrDefault<string>("URL");
            _clients = CrossSettings.Current.GetValueOrDefault<string>("Clients").Split(',').ToList();

            if (_credential == null || _url == null || _clients == null )
            {
                CoreMethods.DisplayAlert("Error", "Setting is incomplete!", "OK");
            }

            else if (_credential != null && _url != null && _clients != null)
            {
                //CalculateReport(_credential, _url, _clients);
                MenuTapped = new Command<string>(NavigateToTaskPage);
            }

            //Push Search Page
            SearchCommand = new Command(async () =>
            {                
                await CoreMethods.PushPageModel<SearchPageModel>();
            });

            //Init Temp
            DaysToComplete = 21.1;
            Task1ChartImage = ImageSource.FromResource("SLB.iTrackr.Resources.RadiusGraph70.png");
            Task2ChartImage = ImageSource.FromResource("SLB.iTrackr.Resources.RadiusGraph65.png");
            Task3ChartImage = ImageSource.FromResource("SLB.iTrackr.Resources.RadiusGraph90.png");
            Task4ChartImage = ImageSource.FromResource("SLB.iTrackr.Resources.RadiusGraph85.png");
            Task5ChartImage = ImageSource.FromResource("SLB.iTrackr.Resources.RadiusGraph60.png");
            Task6ChartImage = ImageSource.FromResource("SLB.iTrackr.Resources.RadiusGraph80.png");
        }

        private async void NavigateToTaskPage(string taskName)
        {
            TaskPageParam pageParam = new TaskPageParam();
            pageParam.Credential = _credential;
            pageParam.URL = _url;
            pageParam.ClientsId = _clients;

            switch(taskName)
            {
                case "Task1":
                    pageParam.Title = "Job Package Completion";                    
                    break;
                case "Task2":
                    pageParam.Title = "Ticket Shipment to Town";
                    break;
                case "Task3":
                    pageParam.Title = "Compile Job Package";
                    break;
                case "Task4":
                    pageParam.Title = "PI Creation";
                    break;
                case "Task5":
                    pageParam.Title = "Client Submission Review";
                    break;
                case "Task6":
                    pageParam.Title = "Invoice Creation";
                    break;
            }

            await CoreMethods.PushPageModel<TaskPageModel>(pageParam);
        }
        
    }
}
