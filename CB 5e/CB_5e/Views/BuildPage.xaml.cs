﻿using CB_5e.ViewModels;
using Character_Builder;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CB_5e.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuildPage : TabbedPage
    {
        private PlayerBuildModel Model;
        public BuildPage(PlayerBuildModel model)
        {
            BindingContext = Model = model;
            InitializeComponent();
            Children.Add(
                    new NavigationPage(new BasePage(Model))
                    {
                        Title = "Base",
                        Icon = Device.OnPlatform("tab_feed.png", null, null)
                    });
            Children.Add(
                    new NavigationPage(new AboutPage())
                    {
                        Title = "Personal",
                        Icon = Device.OnPlatform("tab_about.png", null, null)
                    });
            Children.Add(
                    new NavigationPage(new AboutPage())
                    {
                        Title = "Spell",
                        Icon = Device.OnPlatform("tab_about.png", null, null)
                    });
            Children.Add(
                    new NavigationPage(new ShopPage(Model))
                    {
                        Title = "Item",
                        Icon = Device.OnPlatform("tab_about.png", null, null)
                    });
            Children.Add(
                    new NavigationPage(new AboutPage())
                    {
                        Title = "Config",
                        Icon = Device.OnPlatform("tab_about.png", null, null)
                    });
        }
        protected override bool OnBackButtonPressed()
        {
            if (!Model.ChildModel)
            {
                if (Model.Context.UnsavedChanges > 0)
                {
                    if (DisplayAlert("Unsaved Changes", "You have " + Model.Context.UnsavedChanges + " unsaved changes. Do you want to save them before leaving?", "Yes", "No").Result)
                    {
                        Model.DoSave();
                    }
                }
                CharactersViewModel.Instance.LoadItemsCommand.Execute(null);
            }
            return base.OnBackButtonPressed();
        }
    }
}