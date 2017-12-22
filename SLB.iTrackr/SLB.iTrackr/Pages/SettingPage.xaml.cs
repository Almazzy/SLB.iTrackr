using SLB.iTrackr.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SLB.iTrackr.Models;

namespace SLB.iTrackr.Pages
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<SettingPageModel, List<Client>>(this, "CLIENT_RESULT", (sender, args) => {
                foreach(var client in args)
                {
                    var cellTemp = new SwitchCell();
                    cellTemp.Text = client.Title;
                    cellTemp.On = client.Selected;
                    cellTemp.OnChanged += (s, e) => { client.Selected = e.Value; };

                    ClientListPlaceHolder.Add(cellTemp);
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ClientListPlaceHolder.Clear();

            MessagingCenter.Unsubscribe<SettingPageModel, List<Client>>(this, "CLIENT_RESULT");
        }

    }
}
