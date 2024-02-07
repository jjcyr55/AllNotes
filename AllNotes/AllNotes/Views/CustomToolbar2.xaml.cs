using AllNotes.Views.NewNote.Popups;
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
    public partial class CustomToolbar2 : ContentView
    {
        public CustomToolbar2()
        {
            InitializeComponent();
        }
        public event EventHandler OpenColorPickerRequested;

        protected virtual void OnOpenColorPickerRequested()
        {
            OpenColorPickerRequested?.Invoke(this, EventArgs.Empty);
        }
        private void OpenColorPicker_Clicked(object sender, EventArgs e)
        {
            OnOpenColorPickerRequested();
        }
    }
    }
