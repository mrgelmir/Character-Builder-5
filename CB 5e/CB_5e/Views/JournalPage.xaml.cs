﻿using CB_5e.Helpers;
using CB_5e.ViewModels;
using Character_Builder;
using OGL.Common;
using OGL.Features;
using OGL.Spells;
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
	public partial class JournalPage : CarouselPage
	{
        public PlayerViewModel Model { get; private set; }

        public JournalPage(PlayerViewModel model)
		{
            BindingContext = Model = model;
            InitializeComponent ();
            CurrentPageChanged += CarouselPage_CurrentPageChanged; 
            Title = CurrentPage.Title;
        }

        private async void OnExit(object sender, EventArgs e)
        {
            if (IsBusy) return;
            IsBusy = true;
            if (!Model.ChildModel)
            {
                if (App.AutoSaveDuringPlay)
                {
                    Model.Save();
                    Model.Saving.WaitForAll();
                }
                else if (Model.Context.UnsavedChanges > 0)
                {
                    if (DisplayAlert("Unsaved Changes", "You have " + Model.Context.UnsavedChanges + " unsaved changes. Do you want to save them before leaving?", "Yes", "No").Result)
                    {
                        Model.DoSave();
                    }
                }
            }
            await Navigation.PopModalAsync();
            if (!Model.ChildModel)
            {
                CharactersViewModel.Instance.LoadItemsCommand.Execute(null);
            }
            IsBusy = false;
        }

        private void OnNext(object sender, EventArgs e)
        {
            int i = Children.IndexOf(CurrentPage) + 1;
            if (i < Children.Count) CurrentPage = Children[i];
        }

        private void OnPrev(object sender, EventArgs e)
        {
            int i = Children.IndexOf(CurrentPage) - 1;
            if (i >= 0) CurrentPage = Children[i];
        }

        private void CarouselPage_CurrentPageChanged(object sender, EventArgs e)
        {
            Title = CurrentPage.Title;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new JournalEntryPage(new JournalViewModel(Model)));
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is JournalViewModel je) await Navigation.PushAsync(new JournalEntryPage(je));
            ((ListView)sender).SelectedItem = null;
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            if ((sender as Xamarin.Forms.MenuItem).BindingContext is JournalViewModel obj)
            {
                Model.MakeHistory();
                Model.Context.Player.ComplexJournal.Remove(obj.Journal);
                Model.Save();
                Model.FirePlayerChanged();
            }
        }

        private void MenuItem_Clicked_1(object sender, EventArgs e)
        {
            if ((sender as Xamarin.Forms.MenuItem).BindingContext is string obj)
            {
                Model.MakeHistory();
                Model.Context.Player.Journal.Remove(obj);
                Model.Save();
                Model.RefreshNotes.Execute(null);
            }
        }
    }
}