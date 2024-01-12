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
	}
}