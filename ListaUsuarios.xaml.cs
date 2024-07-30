using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TokenJWT
{
    public partial class ListaUsuarios : ContentPage
    {
        private readonly HttpClient _httpClient;

        public ListaUsuarios()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.0.9:5000") };
            LoadUsuarios();
        }

        private async void LoadUsuarios()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("/usuarios");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(responseBody);
                UsuariosCollectionView.ItemsSource = usuarios;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los usuarios: {ex.Message}", "OK");
            }
        }

        private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;
            var usuario = swipeItem.BindingContext as Usuario;

            if (usuario == null)
                return;

            var confirm = await DisplayAlert("Confirmación", $"¿Estás seguro de que quieres eliminar al usuario '{usuario.usuario}'?", "Sí", "No");
            if (!confirm)
                return;

            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"/usuarios/{usuario.id}");
                response.EnsureSuccessStatusCode();
                await DisplayAlert("Éxito", "Usuario eliminado con éxito", "OK");
                LoadUsuarios();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo eliminar el usuario: {ex.Message}", "OK");
            }
        }

        private async void OnUpdateSwipeItemInvoked(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;
            var usuario = swipeItem.BindingContext as Usuario;

            if (usuario == null)
                return;

            var resultado = await DisplayPromptAsync("Actualizar Usuario", $"Introduce el nuevo nombre de usuario para '{usuario.usuario}'", initialValue: usuario.usuario);
            var nuevaContraseña = await DisplayPromptAsync("Actualizar Contraseña", "Introduce la nueva contraseña", initialValue: usuario.pass);

            if (!string.IsNullOrWhiteSpace(resultado) && !string.IsNullOrWhiteSpace(nuevaContraseña))
            {
                try
                {
                    var usuarioActualizado = new
                    {
                        nuevoUsuario = resultado,
                        nuevaContraseña = nuevaContraseña
                    };

                    var json = JsonConvert.SerializeObject(usuarioActualizado);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await _httpClient.PostAsync($"/usuarios/{usuario.id}", content);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", $"No se pudo actualizar el usuario: {responseContent}", "OK");
                        return;
                    }
                    await DisplayAlert("Éxito", "Usuario actualizado con éxito", "OK");
                    LoadUsuarios();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo actualizar el usuario: {ex.Message}", "OK");
                }
            }
        }


    }

    public class Usuario
    {
        public int id { get; set; } // ID del usuario
        public string usuario { get; set; }
        public string pass { get; set; }
    }
}
