using AllNotes.Droid.CustomRenderers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AllNotes.CustomRenderers;


[assembly: ExportRenderer(typeof(EntryNoUnderlineAndroid), typeof(EntryNoUnderlineAndroid), new[] { typeof(VisualMarker.DefaultVisual) })]


namespace AllNotes.Droid.CustomRenderers
{
    public class EntryNoUnderlineAndroid : EntryRenderer
    {
        public EntryNoUnderlineAndroid(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Background = null;
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}