using AllNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllNotes.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AllNotes.ViewModels;
using Syncfusion.XForms.RichTextEditor;
using System.Collections.ObjectModel;
using AllNotes.Models;
using AllNotes.Database;
using System.ComponentModel;
using System.Net;
using System.IO;
using AllNotes.Views.NewNote.Popups;
using Xamarin.CommunityToolkit.Extensions;

namespace AllNotes.Views.NewNote
{
    public partial class NewNotePage : ContentPage
    {

        public ObservableCollection<AppNote> Notes { get; set; }
        private ObservableCollection<object> _selectedNotes;
        private AppDatabase _appDatabase;
        // private NoteRepository _noteRepository = new NoteRepository();
        private AppNote _note;
        public NewNoteViewModel _newNoteViewModel;
        private MainPageViewModel _mainPageViewModel;
     
        public int FolderID { get; set; } // If needed for specific logic

        // Constructor for a new note in a specific folder
        public NewNotePage(int folderId)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new NewNoteViewModel(folderId);
        }
        /*private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            var content = await webViewRte.EvaluateJavaScriptAsync("document.getElementById('editor').innerHTML;");
            _newNoteViewModel.SaveNoteContent(content);
        }*/
        // Constructor for an existing note
        public NewNotePage(NewNoteViewModel newNoteViewModel)
        {
            InitializeComponent();
            customToolbar.OpenColorPickerRequested += CustomToolbar_OpenColorPickerRequested;
            customToolbar.OpenColorPickerRequested1 += CustomToolbar_OpenColorPickerRequested1;
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = newNoteViewModel;
            SubscribeToMessages();
            newNoteViewModel.FetchWebViewContent = async () => await webViewRte.EvaluateJavaScriptAsync("document.getElementById('editor').innerHTML;");
            
            
            MessagingCenter.Subscribe<NewNoteViewModel>(this, "RequestHtmlContent", async (sender) =>
            {
                var content = await webViewRte.EvaluateJavaScriptAsync("document.getElementById('editor').innerHTML;");
                sender.HtmlContent = content; // Assume HtmlContent is a public property in NewNoteViewModel
            });

            NavigationPage.SetHasNavigationBar(this, false); // Assuming you want to hide the navigation bar
            _newNoteViewModel = newNoteViewModel ?? throw new ArgumentNullException(nameof(newNoteViewModel));
        

            // After ensuring _newNoteViewModel is not null, subscribe to PropertyChanged.
            _newNoteViewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Immediately set the WebView source to ensure it's ready by the time we try to load content into it
            SetWebViewSource(_newNoteViewModel.HtmlContent);
            MessagingCenter.Subscribe<NewNoteViewModel, string>(this, "ExecuteJavaScript", (sender, arg) =>
            {
                webViewRte.EvaluateJavaScriptAsync(arg);
            });
            MessagingCenter.Subscribe<NewNoteViewModel, string>(this, "ExecuteJavaScript", (sender, script) =>
            {
                webViewRte.EvaluateJavaScriptAsync(script).ConfigureAwait(false);
            });
        }
        private void ShowColorPickerPopup()
        {
            var popup = new ColorPickerPopup(); // Assuming this is your custom popup
            popup.BindingContext = new ColorPickerViewModel();
            Application.Current.MainPage.Navigation.ShowPopup(popup);
        }
        private void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<ColorPickerViewModel, string>(this, "ExecuteJavaScript", (sender, script) =>
            {
                webViewRte.EvaluateJavaScriptAsync(script);
            });
        }
        // Default constructor for a new note without specifying a folder
        public NewNotePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new NewNoteViewModel();
        }
        

       
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<NewNoteViewModel, string>(this, "ExecuteJavaScript");
            MessagingCenter.Unsubscribe<NewNoteViewModel, string>(this, "UpdateWebViewContent");
            MessagingCenter.Unsubscribe<NewNoteViewModel, string>(this, "LoadNoteContent");
        }
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_newNoteViewModel.HtmlContent))
            {
                SetWebViewSource(_newNoteViewModel.HtmlContent);
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadNoteContentIntoWebView(_note);
            MessagingCenter.Subscribe<NewNoteViewModel, string>(this, "LoadNoteContent", (sender, content) =>
            {
                webViewRte.Source = new HtmlWebViewSource { Html = content };
            });

           // _newNoteViewModel.LoadNoteContent().ConfigureAwait(false);
            webViewRte.Navigated += WebViewRte_Navigated;
            // Don't load content here since it's now handled after the WebView navigates
            SetWebViewSource();
            MessagingCenter.Subscribe<NewNoteViewModel, string>(this, "ExecuteJavaScript", (sender, script) =>
            {
                webViewRte.EvaluateJavaScriptAsync(script);
            });
            // Optionally, load content directly if the WebView is already ready
            if (_note != null && _note.Text != null)
            {
                LoadContentIntoWebView(_note.Text);
            }
        }
       
        

        private void LoadNoteContentIntoWebView(AppNote note)
        {
            if (note != null && !string.IsNullOrEmpty(note.Text))
            {
                webViewRte.Source = new HtmlWebViewSource
                {
                    Html = note.Text
                };
            }
        }
        /*private void CustomToolbar_OpenColorPickerRequested(object sender, EventArgs e)
        {
            var colorPickerPopup = new ColorPickerPopup(webViewRte);
            Application.Current.MainPage.Navigation.ShowPopup(colorPickerPopup);
        }*/
        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            var content = await webViewRte.EvaluateJavaScriptAsync("document.getElementById('editor').innerHTML;");
            // Assuming you have a command bound to your save button
            if (BindingContext is NewNoteViewModel viewModel)
            {
                viewModel.SaveNoteCommand.Execute(content);
            }
        }
        private void CustomToolbar_OpenColorPickerRequested(object sender, EventArgs e)
        {
            var colorPickerPopup = new ColorPickerPopup(webViewRte);
            Application.Current.MainPage.Navigation.ShowPopup(colorPickerPopup);
        }
        private void CustomToolbar_OpenColorPickerRequested1(object sender, EventArgs e)
        {
            var colorPickerPopup1 = new ColorPickerPopup1(webViewRte);
            Application.Current.MainPage.Navigation.ShowPopup(colorPickerPopup1);
        }
        /*private void CustomToolbar_OpenColorPickerRequested1(object sender, EventArgs e)
        {
            var colorPickerPopup1 = new ColorPickerPopup1(webViewRte);
            Application.Current.MainPage.Navigation.ShowPopup(colorPickerPopup1);
        }*/

        private void WebViewRte_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_newNoteViewModel.HtmlContent))
            {
                var decodedContent = WebUtility.HtmlDecode(_newNoteViewModel.HtmlContent);
                webViewRte.EvaluateJavaScriptAsync($"setContent(`{decodedContent}`);");
            }
        }

        private void LoadContentIntoWebView(string content)
        {

            var escapedContent = Uri.EscapeDataString(content);
            var script = $"document.getElementById('editor').innerHTML = decodeURIComponent('{escapedContent}');";
            webViewRte.EvaluateJavaScriptAsync(script).ConfigureAwait(false); // Added ConfigureAwait to avoid deadlocks

        }
        /*private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            var content = await webViewRte.EvaluateJavaScriptAsync("document.getElementById('editor').innerHTML;");
            _newNoteViewModel.SaveNoteContent(content);
        }*/

        private void SetWebViewSource(string content = null)
        {
            if (string.IsNullOrEmpty(content))
            {
                // If no content is provided, load the HTML file as before
                var baseUrl = DependencyService.Get<IBaseUrlGetter>().GetBaseUrl();
                webViewRte.Source = new UrlWebViewSource { Url = Path.Combine(baseUrl, "RichTextEditor.html") };
            }
            else
            {
                // If content is provided, load it directly into the WebView
                webViewRte.Source = new HtmlWebViewSource { Html = content };
            }
        }

    }
}