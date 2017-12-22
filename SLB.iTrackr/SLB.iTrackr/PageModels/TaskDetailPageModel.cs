using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreshMvvm;
using SLB.iTrackr.Models;
using PropertyChanged;
using System.Windows.Input;
using Xamarin.Forms;
using SLB.iTrackr.Utils;
using Newtonsoft.Json.Linq;
using Plugin.Settings;

namespace SLB.iTrackr.PageModels
{
    [ImplementPropertyChanged]
    public class TaskDetailPageModel : FreshBasePageModel
    {
        public Ticket Ticket { get; set; }
        public bool TaskDone { get; set; }
        public DateTime DueDate { get; set; }
        public int JPShipmentTypeIndex { get; set; }
        public ICommand SaveCommand { get; set; }
        public bool IsBusy { get; set; }
        public bool IsNotBusy { get { return !IsBusy; } set { IsNotBusy = value; } }

        public bool TicketValueEnable { get; set; }
        public bool CurrencyEnable { get; set; }
        public bool JobTypeEnable { get; set; }
        public bool JobDateEnable { get; set; }
        public bool ShipmentEnable { get; set; }
        public bool ShipmentVisible { get; set; }
        public bool FTLEnable { get; set; }

        private Credential _credential;
        private string _url;

        public TaskDetailPageModel()
        {

        }

        public override void Init(object initData)
        {
            base.Init(initData);

            //Init Field Enable
            TicketValueEnable = false;
            CurrencyEnable = false;
            JobTypeEnable = false;
            JobDateEnable = false;
            ShipmentEnable = false;
            ShipmentVisible = true;
            FTLEnable = false;


            var t = (Ticket)initData;            

            if(t != null)
            {
                Ticket = t;

                switch (t.TicketStatus) 
                {
                    case "Job Package Completion":
                        TaskDone = Ticket.Task1Done;
                        DueDate = (DateTime)Ticket.Task1DueDate;
                        TicketValueEnable = true;
                        CurrencyEnable = true;
                        JobTypeEnable = true;
                        JobDateEnable = true;
                        ShipmentVisible = false;
                        FTLEnable = true;
                        break;
                    case "Ticket Shipment to Town":
                        TaskDone = Ticket.Task2Done;
                        DueDate = (DateTime)Ticket.Task2DueDate;
                        ShipmentEnable = true;
                        FTLEnable = true;
                        break;
                    case "Compile Job Package":
                        TaskDone = Ticket.Task3Done;
                        DueDate = (DateTime)Ticket.Task3DueDate;
                        break;
                    case "PI Creation":
                        TaskDone = Ticket.Task4Done;
                        DueDate = (DateTime)Ticket.Task4DueDate;
                        break;
                    case "Client Submission Review":
                        TaskDone = Ticket.Task5Done;
                        DueDate = (DateTime)Ticket.Task5DueDate;
                        break;
                    case "Invoice Creation":
                        TaskDone = Ticket.Task6Done;
                        DueDate = (DateTime)Ticket.Task6DueDate;
                        break;
                    case "Finish":
                        TaskDone = true;
                        break;
                }
            }

            if(t.JPShipmentType != null)
            {
                switch(t.JPShipmentType)
                {
                    case "Hand Carry":
                        JPShipmentTypeIndex = 0;
                        break;
                    case "JNE":
                        JPShipmentTypeIndex = 1;
                        break;
                    case "TIKI":
                        JPShipmentTypeIndex = 2;
                        break;
                    case "DHL":
                        JPShipmentTypeIndex = 3;
                        break;
                    default:
                        JPShipmentTypeIndex = -1;
                        break;
                }
            }

            _credential = DependencyService.Get<ICredentialService>().GetCredential();
            _url = CrossSettings.Current.GetValueOrDefault<string>("URL");

            if (_credential != null && _url != null)
            {
                SaveCommand = new Command(Save);
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
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            MessagingCenter.Unsubscribe<SPService, string>(this, "SP_REQUEST_STATUS");
        }

        private async void Save()
        {

            switch(JPShipmentTypeIndex)
            {   
                case 0:
                    Ticket.JPShipmentType = "Hand Carry";
                    break;
                case 1:
                    Ticket.JPShipmentType = "JNE";
                    break;
                case 2:
                    Ticket.JPShipmentType = "TIKI";
                    break;
                case 3:
                    Ticket.JPShipmentType = "DHL";
                    break;
            }            

            if(TaskDone == true)
            {
                switch(Ticket.TicketStatus)
                {
                    case "Job Package Completion":                                                
                        Ticket.TicketStatus = "Ticket Shipment to Town";
                        break;
                    case "Ticket Shipment to Town":                        
                        Ticket.TicketStatus = "Compile Job Package";
                        break;
                    case "Compile Job Package":
                        Ticket.TicketStatus = "PI Creation";
                        break;
                    case "PI Creation":
                        Ticket.TicketStatus = "Client Submission Review";
                        break;
                    case "Client Submission Review":
                        Ticket.TicketStatus = "Invoice Creation";
                        break;
                    case "Invoice Creation":
                        Ticket.TicketStatus = "Finish";
                        break;
                }
            }

            JObject body = new JObject();
            body.Add(new JProperty("FTLNumber", Ticket.FTLNumber));
            body.Add(new JProperty("JobStartDate", ((DateTime)Ticket.JobStartDate).ToString("s")));
            body.Add(new JProperty("JobEndDate", ((DateTime)Ticket.JobEndDate).ToString("s")));
            body.Add(new JProperty("CurrencyValue", Ticket.Currency));
            body.Add(new JProperty("TicketValue", Ticket.TicketValue));
            body.Add(new JProperty("JobType", Ticket.JobType));
            body.Add(new JProperty("JPShipmentDate", ((DateTime)Ticket.JPShipmentDate).ToString("s")));
            body.Add(new JProperty("JPShipmentTypeValue", Ticket.JPShipmentType));
            body.Add(new JProperty("Remarks", Ticket.Remarks));       
            body.Add(new JProperty("TicketStatusValue", Ticket.TicketStatus));
            
            var sp = new SPService(_credential, _url);

            IsBusy = true;
            await sp.SaveTicket(Ticket.Id, body);
            IsBusy = false;

            await CoreMethods.PopToRoot(true);
            
        }        
    }    
}
