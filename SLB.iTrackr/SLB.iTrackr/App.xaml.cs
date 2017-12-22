using System;
using SLB.iTrackr.Navigation;
using Xamarin.Forms;
using FreshMvvm;
using SLB.iTrackr.Models;
using SLB.iTrackr.Utils;
using SLB.iTrackr.PageModels;

namespace SLB.iTrackr
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new CustomNavigation();
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
