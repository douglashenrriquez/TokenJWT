using Microsoft.Maui.Controls;

namespace TokenJWT
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            //MainPage = new NavigationPage(new ListaUsuarios());
        }
    }
}
