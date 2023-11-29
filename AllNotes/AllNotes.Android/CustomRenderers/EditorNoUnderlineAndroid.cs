using AllNotes.CustomRenderers;
using Android.Content;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AllNotes.CustomRenderers;
//using AllNotes.Droid.CustomRenderers;
using XamarinNoteApp.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(EditorNoUnderline), typeof(EditorNoUnderlineAndroid), new[] { typeof(VisualMarker.DefaultVisual) })]

namespace XamarinNoteApp.Droid.CustomRenderers
{
    public class EditorNoUnderlineAndroid : EditorRenderer
    {
        public EditorNoUnderlineAndroid(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.Background = null;
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
                Control.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                if (e.NewElement is EditorNoUnderline newElement)
                {
                    if (newElement.Alignment == 0)
                        Control.Gravity = Android.Views.GravityFlags.Start;
                    else if (newElement.Alignment == 1)
                        Control.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                    else
                        Control.Gravity = Android.Views.GravityFlags.End;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == EditorNoUnderline.AlignmentProperty.PropertyName)
            {
                var editor = sender as EditorNoUnderline;
                if (editor.Alignment == 0)
                    Control.Gravity = Android.Views.GravityFlags.Start;
                else if (editor.Alignment == 1)
                    Control.Gravity = Android.Views.GravityFlags.CenterHorizontal;
                else
                    Control.Gravity = Android.Views.GravityFlags.End;
                Control.Invalidate();
            }
        }
    }
}