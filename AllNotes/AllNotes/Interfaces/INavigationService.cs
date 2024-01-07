using AllNotes.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllNotes.Interfaces
{
    public interface INavigationService
    {
        Task NavigateToMainPage(AppFolder selectedFolder);
       // Task NavigateToNewNotePage(int folderID);
        
        // You can add other navigation methods as needed
    }
}
