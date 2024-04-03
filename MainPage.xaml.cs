using CodeCadence.Maui.Fireworks;

namespace FireworkExperiment
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            AnimationView.Firework = new(AnimationView);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            AnimationView.Firework.Start(TimeSpan.FromMilliseconds(10));
        }
    }
}
