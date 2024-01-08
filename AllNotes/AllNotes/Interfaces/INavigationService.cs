using AllNotes.Models;
using AllNotes.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllNotes.Interfaces
{
    public interface INavigationService
    {
         Task NavigateTo(ManageFolders manageFoldersPage);
      
        Task NavigateToMainPage(AppFolder selectedFolder);
       // Task NavigateToNewNotePage(int folderID);
        
        // You can add other navigation methods as needed
    }
}
