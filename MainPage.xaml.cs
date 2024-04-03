using CodeCadence.Maui.Fireworks;

namespace FireworkExperiment
{
    
    public partial class MainPage : ContentPage
    {
        MainViewModel _model = new();

        public MainPage()
        {
            BindingContext = _model;
            InitializeComponent();
            AnimationView.Firework = new(AnimationView);
        }

        protected override void OnDisappearing()
        {
            AnimationView.Firework.Stop();
            base.OnDisappearing();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            AnimationView.Firework.Start((int)_model.Framerate);
        }
    }
}
