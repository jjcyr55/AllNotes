using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AllNotes.CustomRenderers
{
    public class EditorNoUnderline : Editor
    {
        public static readonly BindableProperty AlignmentProperty =
            BindableProperty.Create(nameof(Alignment),
                typeof(int),
                typeof(EditorNoUnderline),
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: AlignmentPropertyChanged);

        private static void AlignmentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var editor = bindable as EditorNoUnderline;
            editor.Alignment = (int)newValue;
        }

        public int Alignment
        {
            get { return (int)GetValue(AlignmentProperty); }
            set { SetValue(AlignmentProperty, value); }
        }

        public EditorNoUnderline()
        {
        }
    }
}