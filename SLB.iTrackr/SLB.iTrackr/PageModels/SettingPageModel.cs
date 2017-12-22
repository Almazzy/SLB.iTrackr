using System;
using FreshMvvm;
using PropertyChanged;
using SLB.iTrackr.Models;
using SLB.iTrackr.Utils;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.Generic;
using Plugin.Settings;
using System.Threading.Tasks;
using System.Linq;

namespace SLB.iTrackr.PageModels
{
    [ImplementPropertyChanged]
    public class SettingPageModel : FreshBasePageModel
    {
        public string URL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsBusy { get; set; }
        public List<Client> Clients { get; set; }
        public ICommand SaveConfigsCommand { get; set; }

        public SettingPageModel()
        {
           
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            SaveConfigsCommand = new Command(SaveConfiguration);        
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);            

            //Get REST request status
            MessagingCenter.Subscribe<SPService, string>(this, "SP_REQUEST_STATUS", (s, args) =>
            {
                CoreMethods.DisplayAlert("Notification", args, "OK");
            });

            IsBusy = false;

            var credTemp = DependencyService.Get<ICredentialService>().GetCredential();
            var urlTemp = CrossSettings.Current.GetValueOrDefault<string>("URL");
            var clientsTemp = CrossSettings.Current.GetValueOrDefault<string>("Clients");

            if (credTemp != null)
            {
                UserName = credTemp.UserName;
                Password = credTemp.Password;
            }            

            if (urlTemp != null)
                URL = urlTemp;

            if(credTemp != null && urlTemp != null)
                GetClients(clientsTemp);
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            //Unsubscribe REST request status
            MessagingCenter.Unsubscribe<SPService, string>(this, "SP_REQUEST_STATUS");
        }

        private void SaveConfiguration()
        {
            //Save Credential
            bool saveCredSuccess = DependencyService.Get<ICredentialService>().SaveCredential(new Credential
            {
                UserName = UserName,
                Password = Password
            });

            //Save User Preference
            bool saveConfSuccess = CrossSettings.Current.AddOrUpdateValue("URL", URL);            

            //Check Selection
            if (Clients != null)
            {
                var listClientTemp = new List<int>();

                foreach (var c in Clients)
                {
                    if (c.Selected == true)
                        listClientTemp.Add(c.Id);
                }

                string listClientsJoined = string.Join(",", listClientTemp);

                //Save Client Selection
                saveConfSuccess = CrossSettings.Current.AddOrUpdateValue("Clients", listClientsJoined);
            }            

            //Notify is success
            if (saveCredSuccess && saveConfSuccess)
            {
                CoreMethods.DisplayAlert("Notification", "Configuration Saved!", "OK");
            }
            else
            {
                CoreMethods.DisplayAlert("Notification", "Cannot Save Configuration!", "OK");
            }
        }

        private async void GetClients(string clientListId)
        {
            var listClientTemp = clientListId.Split(',').ToList();        

            var sp = new SPService(new Credential { UserName = UserName, Password = Password }, URL);

            IsBusy = true;
            Clients = await sp.GetClients();
            IsBusy = false;

            if(Clients != null)
            {
                foreach (var c in Clients)
                {
                    if (listClientTemp.Exists(Id => Id == c.Id.ToString()))
                    {
                        c.Selected = true;
                    }
                    else
                        c.Selected = false;
                }
                
                MessagingCenter.Send<SettingPageModel, List<Client>>(this, "CLIENT_RESULT", Clients);
            }            
            
        }
      
    }
}
