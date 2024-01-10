using Xamarin.Forms;

namespace AllNotes.Animations
{
    internal class PopupAnimation
    {
        public double OpacityIn { get; set; }
        public double OpacityOut { get; set; }
        public double ScaleIn { get; set; }
        public double ScaleOut { get; set; }
        public int DurationIn { get; set; }
        public int DurationOut { get; set; }
        public Easing EasingIn { get; set; }
        public Easing EasingOut { get; set; }
    }
}