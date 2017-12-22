using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;
using FreshMvvm;
using PropertyChanged;

using SLB.iTrackr.PageModels;
using SLB.iTrackr.Configs;
using SLB.iTrackr.Models;

namespace SLB.iTrackr.Navigation
{
    public class CustomNavigation : MasterDetailPage, IFreshNavigationService
    {
        private FreshNavigationContainer _homeNav, _archiveNav, _reportNav, _settingNav;

        public CustomNavigation()
        {
            NavigationServiceName = "CustomNavigation";
            SetupAllPagesNav();
            
            CreateMasterPage("Menu");
            //this.Detail = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<HomePageModel>(), "HomePageNav");
            RegisterNavigation();
        }

        public string NavigationServiceName { get; private set; }

        protected void RegisterNavigation()
        {
            FreshIOC.Container.Register<IFreshNavigationService>(this, NavigationServiceName);
        }

        protected void SetupAllPagesNav()
        {
            _homeNav = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<HomePageModel>(), "HomePageNav");
            _archiveNav = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<ArchivePageModel>(), "ArchivePageNav");
            _reportNav = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<ReportPageModel>(), "ArchivePageNav");
            _settingNav = new FreshNavigationContainer(FreshPageModelResolver.ResolvePageModel<SettingPageModel>(), "ArchivePageNav");
        }

        protected void CreateMasterPage(string title)
        {
            var masterPage = new ContentPage();
            masterPage.Title = title;

            /*if (Device.OS == TargetPlatform.iOS)
                masterPage.Padding = new Thickness(0, 30, 0, 0);*/

            var menuListViewItems = new List<MenuItem>()
            {
                new MenuItem("Home", ImageSource.FromResource("SLB.iTrackr.Resources.MenuHome.png")),
                new MenuItem("Archive", ImageSource.FromResource("SLB.iTrackr.Resources.MenuArchive.png")),
                new MenuItem("Report", ImageSource.FromResource("SLB.iTrackr.Resources.MenuReport.png")),
                new MenuItem("Setting", ImageSource.FromResource("SLB.iTrackr.Resources.MenuSetting.png")),
                new MenuItem("About", ImageSource.FromResource("SLB.iTrackr.Resources.MenuAbout.png"))
            };

            #region __Master Page Layout__
            
            var menuListView = new ListView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                SeparatorVisibility = SeparatorVisibility.None,
                RowHeight = 60,
                ItemsSource = menuListViewItems,
                ItemTemplate = new DataTemplate(() => 
                {
                    var menuIcon = new Image {Aspect = Aspect.AspectFit, VerticalOptions = LayoutOptions.Center};
                    menuIcon.SetBinding(Image.SourceProperty, "IconSource");

                    var rightArrowIcon = new Image 
                    { 
                        Aspect = Aspect.AspectFit, 
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        Source = ImageSource.FromResource("SLB.iTrackr.Resources.RightArrow.png")
                    };                    

                    var menuLabel = new Label
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center
                    };
                    menuLabel.SetBinding(Label.TextProperty, "Title");
                    menuLabel.SetBinding(Label.TextColorProperty, "TextColor");
                    
                    var cellStack = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(20, 10, 0, 10),
                        Spacing = 20,
                        Children = {menuIcon, menuLabel, rightArrowIcon}
                    };
                    
                    return new ViewCell { View = cellStack };
                })
            };

            menuListView.ItemSelected += (async (sender, args) => 
            {                
                //Reset Selection Color             
                foreach (var menuItem in menuListViewItems)
                {
                    menuItem.TextColor = Color.FromHex(ColorScheme.PrimaryColor);
                }

                MenuItem selected = (MenuItem)args.SelectedItem;
                selected.TextColor = Color.FromHex(ColorScheme.AccentColor);

                switch((string) selected.Title)
                {
                    case "Home":
                        //await PopToRoot();
                        this.Detail = _homeNav;
                        IsPresented = false;
                        break;
                    case "Archive":
                        //await PushPage(FreshPageModelResolver.ResolvePageModel<ArchivePageModel>(), null);
                        this.Detail = _archiveNav;
                        IsPresented = false;
                        break;
                    case "Report":
                        //await PushPage(FreshPageModelResolver.ResolvePageModel<ReportPageModel>(), null);
                        this.Detail = _reportNav;
                        IsPresented = false;
                        break;
                    case "Setting":
                        //await PushPage(FreshPageModelResolver.ResolvePageModel<SettingPageModel>(), null);
                        this.Detail = _settingNav;
                        IsPresented = false;
                        break;
                    case "About":
                        menuListView.SelectedItem = menuListViewItems.Find(Menu => Menu.Title == "Home");
                        await PushPage(FreshPageModelResolver.ResolvePageModel<AboutPageModel>(), null, true);
                        IsPresented = false;
                        break;
                }                
            });

            var masterPageHeader = new Image
            {
                HeightRequest = 150,
                Aspect = Aspect.AspectFit,
                Source = ImageSource.FromResource("SLB.iTrackr.Resources.iTrackrLogo.png"),
                BackgroundColor = Color.FromHex(ColorScheme.PrimaryColor)
            };

            var masterPageFooter = new Image
            {
                HeightRequest = 50,
                Aspect = Aspect.AspectFit,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Source = ImageSource.FromResource("SLB.iTrackr.Resources.MenuNewTicket.png"),
                BackgroundColor = Color.FromHex(ColorScheme.PrimaryColor)
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                menuListView.SelectedItem = menuListViewItems.Find(Menu => Menu.Title == "Home");
                var newTicketPage = FreshPageModelResolver.ResolvePageModel<NewTicketPageModel>();
                await PushPage(newTicketPage, null);
                IsPresented = false;                
            };

            masterPageFooter.GestureRecognizers.Add(tapGestureRecognizer);

            var mainStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    masterPageHeader,
                    menuListView,
                    masterPageFooter
                }
            };            

            #endregion

            masterPage.Content = mainStack;       
            Master = masterPage;

            //Initial Selection on Master Menu
            menuListView.SelectedItem = menuListViewItems.Find(Menu => Menu.Title == "Home");
        }      

        public virtual async Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                await ((FreshNavigationContainer)this.Detail).PopPage(true, animate);
            else
                await ((FreshNavigationContainer)this.Detail).PopPage(modal, animate);
        }

        public virtual async Task PopToRoot(bool animate = true)
        {
            await ((FreshNavigationContainer)this.Detail).PopToRoot(animate);
        }

        public virtual async Task PushPage(Page page, FreshBasePageModel model, bool modal = false, bool animate = true)
        {
            if (modal)
                await ((FreshNavigationContainer)this.Detail).PushPage(page, null, true, animate);
            else
                await ((FreshNavigationContainer)this.Detail).PushPage(page, null, modal, animate);
        }

                public void NotifyChildrenPageWasPopped()
        {
            throw new NotImplementedException();
        }

        public Task<FreshBasePageModel> SwitchSelectedRootPageModel<T>() where T : FreshBasePageModel
        {
            throw new NotImplementedException();
        }

        /*private class MenuItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string Title { get; set; }            
            public ImageSource IconSource { get; private set; }

            private Color _textColor;
            public Color TextColor 
            {
                get { return _textColor; }
                set 
                {
                    _textColor = value;

                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("TextColor"));
                }
            }

            public MenuItem(string title, ImageSource iconSource)
            {
                Title = title;
                IconSource = iconSource;
                TextColor = Color.FromHex(ColorScheme.PrimaryColor);
            }
            
        }*/

        [ImplementPropertyChanged]
        private class MenuItem
        {
            public string Title { get; set; }
            public ImageSource IconSource { get; set; }
            public Color TextColor { get; set; }

            public MenuItem(string title, ImageSource iconSource)
            {
                Title = title;
                IconSource = iconSource;
                TextColor = Color.FromHex(ColorScheme.PrimaryColor);
            }
        }


    }
}
