﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllNotes.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes
{
   // [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewFolderPage : ContentPage
    {
        private NewFolderViewModel newFolderViewModel;

        public NewFolderPage()
        {
            InitializeComponent();
           // this.newFolderViewModel = newFolderViewModel;
        }


        public NewFolderPage(NewFolderViewModel newFolderViewModel)
        {
            InitializeComponent();
            this.newFolderViewModel = newFolderViewModel;
        }

        public NewFolderPage(NewFolderPage newFolderPage)
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {

        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {

        }
    }
}