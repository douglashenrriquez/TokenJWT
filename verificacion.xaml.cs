using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TokenJWT
{
    public partial class verificacion : ContentPage
    {
        private readonly HttpClient _httpClient;

        // Define la base URL directamente en la clase
        private const string BaseUrl = "http://192.168.1.8:5000";

        public verificacion()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        private async void OnVerifyButtonClicked(object sender, EventArgs e)
        {
            string token = tokenEntry.Text;

            if (string.IsNullOrWhiteSpace(token))
            {
                await DisplayAlert("Error", "Token es requerido", "OK");
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/info");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Token válido, navegar a la página ListaUsuarios
                    await Navigation.PushAsync(new ListaUsuarios());
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
