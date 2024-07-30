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
            _httpClient = new HttpClient { BaseAddress = new Uri("http://192.168.1.8:5000") };
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

            var confirm = await DisplayAlert("Confirmaci�n", $"�Est�s seguro de que quieres eliminar al usuario '{usuario.usuario}'?", "S�", "No");
            if (!confirm)
                return;

            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"/usuarios/{usuario.id}");
                response.EnsureSuccessStatusCode();
                await DisplayAlert("�xito", "Usuario eliminado con �xito", "OK");
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
            var nuevaContrase�a = await DisplayPromptAsync("Actualizar Contrase�a", "Introduce la nueva contrase�a", initialValue: usuario.pass);

            if (!string.IsNullOrWhiteSpace(resultado) && !string.IsNullOrWhiteSpace(nuevaContrase�a))
            {
                try
                {
                    var usuarioActualizado = new Usuario
                    {
                        usuario = resultado,
                        pass = nuevaContrase�a
                    };

                    var json = JsonConvert.SerializeObject(usuarioActualizado);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await _httpClient.PutAsync($"/usuarios/{usuario.id}", content);
                    response.EnsureSuccessStatusCode();
                    await DisplayAlert("�xito", "Usuario actualizado con �xito", "OK");
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
