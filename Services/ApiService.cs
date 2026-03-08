using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetCatFactAsync()
        {
            // Llamamos a la API de gatos pidiendo el dato en español (lang=esp)
            var response = await _httpClient.GetFromJsonAsync<CatFactResponse>("https://meowfacts.herokuapp.com/?lang=esp");

            // Devolvemos solo la primera frase de la lista
            return response?.Data?.FirstOrDefault() ?? "No hay consejos disponibles hoy.";
        }
    }
}