using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CustomToolbar : ContentView
	{
		public CustomToolbar ()
		{
			InitializeComponent ();
		}
        private void OnMoreTapped(object sender, EventArgs e)
        {
            // Implement the action to be performed when the 'More' label is tapped
        }
    }
}