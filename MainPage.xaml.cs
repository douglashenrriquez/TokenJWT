using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TokenJWT
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        // Define the base URL directly in the MainPage class
        private const string BaseUrl = "http://192.168.1.8:5000";

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            string usuario = userEntry.Text;
            string pass = passEntry.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(pass))
            {
                await DisplayAlert("Error", "Usuario y contraseña son requeridos", "OK");
                return;
            }

            var registerData = new
            {
                usuario = usuario,
                pass = pass
            };

            string json = JsonConvert.SerializeObject(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync("/register", content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Success", "Usuario registrado exitosamente", "OK");

                    // Limpiar los campos después de enviar los datos
                    userEntry.Text = "";
                    passEntry.Text = "";

                    // Navegar a la página de verificación
                    await Navigation.PushAsync(new verificacion());
                }
                else
                {
                    await DisplayAlert("Error", $"Error: {responseContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
            }
        }
    }
}
