﻿using AllNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllNotes.Views.NewNote.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPickerPopup : Popup
    {
        private WebView _webViewRte;

        /*public ColorPickerPopup(WebView webViewRte)
        {
             BindingContext = new ColorPickerViewModel();
              MessagingCenter.Unsubscribe<NewNoteViewModel>(this, "ExecuteJavaScript");

              _webView = webViewRte;
        }*/
        public ColorPickerPopup(WebView webViewRte) : this() // Call the default constructor
        {
            _webViewRte = webViewRte;
            BindingContext = new ColorPickerViewModel();
            // Additional initialization with WebView if necessary
        }
        public ColorPickerPopup()
        {
            InitializeComponent();

            BindingContext = new ColorPickerViewModel();
        }
        /*private void OnRedButtonClicked(object sender, EventArgs e)
        {
            _webViewRte?.EvaluateJavaScriptAsync("setTextColor('#FF0000')");
        }*/
        /*public ColorPickerPopup()
        {
        }*/
        public event EventHandler<Color> ColorSelected;

        // Method to invoke the ColorSelected event
        protected virtual void OnColorSelected(Color color)
        {
            ColorSelected?.Invoke(this, color);
        }
        private void OnSetTextColorRedClicked(object sender, EventArgs e)
        {
            _webViewRte?.EvaluateJavaScriptAsync("setTextColor('#FF0000')");
        }
        /*private void RedButton_Clicked(object sender, EventArgs e)
        {
            OnColorSelected(Color.Red);

        }

        private void GreenButton_Clicked(object sender, EventArgs e)
        {
            OnColorSelected(Color.Green);

        }

        private void BlueButton_Clicked(object sender, EventArgs e)
        {
            OnColorSelected(Color.Blue);

        }*/
        /*public void OnSetTextColorRedClicked(object sender, EventArgs e)
        {
            ExecuteJavaScript("setTextColor('#FF0000')");
        }*/

        public void OnSetTextColorGreenClicked(object sender, EventArgs e)
        {
            ExecuteJavaScript("setTextColor('#00FF00')");
        }

        public void OnSetTextColorBlueClicked(object sender, EventArgs e)
        {
            ExecuteJavaScript("setTextColor('#0000FF')");
        }

        private void ExecuteJavaScript(string script)
        {
            // Assuming 'webView' is the WebView that hosts your rich text editor
            // You'll need to find a way to reference your WebView here. It could be through a static reference, 
            // passing the WebView instance to the popup when it's created, or another method suitable for your app's architecture.
            _webViewRte?.EvaluateJavaScriptAsync(script);
        }
        private void OnColorButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && _webViewRte != null)
            {
                var color = button.BackgroundColor.ToHex();
                var script = $"setTextColor('{color}')";
                _webViewRte.EvaluateJavaScriptAsync(script);
            }
        }
        private void OnRedButtonClicked(object sender, EventArgs e)
        {
            _webViewRte.EvaluateJavaScriptAsync("setTextColor('#FF0000')");
            this.Dismiss(null);
        }
        /*private void OnRedButtonClicked(object sender, EventArgs e)
        {
            _webViewRte?.EvaluateJavaScriptAsync("setTextColor('#FF0000')");
            this.Dismiss(null); // Close the popup after setting the color
        }*/
        private void OnGreenButtonClicked(object sender, EventArgs e)
        {
            _webViewRte.EvaluateJavaScriptAsync("setTextColor('#00FF00')");
            this.Dismiss(null);
        }

        private void OnBlueButtonClicked(object sender, EventArgs e)
        {
            _webViewRte.EvaluateJavaScriptAsync("setTextColor('#0000FF')");
            this.Dismiss(null);
        }
        private void OnYellowButtonClicked(object sender, EventArgs e)
        {
            _webViewRte.EvaluateJavaScriptAsync("setTextColor('#FFFF00')");
            this.Dismiss(null);
        }

        private void OnPurpleButtonClicked(object sender, EventArgs e)
        {
            _webViewRte.EvaluateJavaScriptAsync("setTextColor('#800080')");
            this.Dismiss(null);
        }

        private void OnOrangeButtonClicked(object sender, EventArgs e)
        {
            _webViewRte.EvaluateJavaScriptAsync("setTextColor('#FFA500')");
            this.Dismiss(null);
        }

        private void OnBlackButtonClicked(object sender, EventArgs e)
        {
            _webViewRte.EvaluateJavaScriptAsync("setTextColor('#000000')");
            this.Dismiss(null);
        }

        private void OnWhiteButtonClicked(object sender, EventArgs e)
        {
            _webViewRte.EvaluateJavaScriptAsync("setTextColor('#FFFFFF')");
            this.Dismiss(null);
        }
        private void ColorPicker_ColorChanged(object sender, EventArgs e)
        {
            // e.Color is your selected color
            MessagingCenter.Send(this, "ColorSelected");
        }
    }
}