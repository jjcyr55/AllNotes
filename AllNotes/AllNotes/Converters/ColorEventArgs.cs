using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AllNotes.Converters
{
    public class ColorEventArgs : EventArgs
    {
        public Color SelectedColor { get; set; }

        public ColorEventArgs(Color color)
        {
            SelectedColor = color;
        }
    }
}
