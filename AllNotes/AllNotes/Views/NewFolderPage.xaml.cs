using System;
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
        /* private NewFolderViewModel newFolderViewModel;

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

         private async void Button_Clicked(object sender, EventArgs e)
         {
             // Display an input prompt to get the new folder name
             string newFolderName = await DisplayPromptAsync("New Folder", "Enter a name for the new folder:", "Create", "Cancel");

             if (!string.IsNullOrWhiteSpace(newFolderName))
             {
                 // Create a new folder and add it to the view model or database
                 // Example: newFolderViewModel.AddNewFolder(newFolderName);
                 // Make sure to implement the necessary logic in your NewFolderViewModel

                 // Close the page or navigate back
                 await Navigation.PopAsync();
             }
         }

         private void Button_Clicked_1(object sender, EventArgs e)
         {

         }

         private void Button_Clicked_2(object sender, EventArgs e)
         {

         }
     }*/
    }
}