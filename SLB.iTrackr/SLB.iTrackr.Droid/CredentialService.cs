using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SLB.iTrackr.Models;
using SLB.iTrackr.Utils;
using SLB.iTrackr.Droid;

using Xamarin.Auth;
using Xamarin.Forms;


[assembly: Xamarin.Forms.Dependency (typeof(CredentialService))]
namespace SLB.iTrackr.Droid
{
    public class CredentialService : ICredentialService
    {
        private string _appName = "SLB.iTrackr";

        public bool SaveCredential(Credential credential)
        {
            if (!string.IsNullOrWhiteSpace(credential.UserName) &&
                !string.IsNullOrWhiteSpace(credential.Password))                
            {
                ClearCredential();

                Account account = new Account { Username = credential.UserName };
                account.Properties.Add("Password", credential.Password);

                try
                {
                    AccountStore.Create(Forms.Context).Save(account, _appName);
                }
                catch (System.Exception)
                {
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        public Credential GetCredential()
        {
            IEnumerable<Account> accounts = AccountStore.Create(Forms.Context).FindAccountsForService(_appName);

            if (accounts.Count() > 0)
            {
                var account = accounts.FirstOrDefault();

                return (new Credential
                {
                    UserName = account.Username.ToString(),
                    Password = account.Properties["Password"].ToString()
                });
            }
            else
                return null;
        }

        private void ClearCredential()
        {
            IEnumerable<Account> accounts = AccountStore.Create(Forms.Context).FindAccountsForService(_appName);

            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    AccountStore.Create(Forms.Context).Delete(account, _appName);
                }
            }          
        }
    }
}