using ClimaTemp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Globalization;

namespace ClimaTemp.Controllers
{
    public class HomeController : Controller
    {
        private string ObterIconeClima(int wmoCode)
        {
            return wmoCode switch
            {
                0 => "☀️", // Céu limpo
                1 or 2 or 3 => "⛅", // Nublado / Parcialmente nublado
                45 or 48 => "🌫️", // Nevoeiro
                51 or 53 or 55 or 56 or 57 => "🌦️", // Chuvisco
                61 or 63 or 65 or 66 or 67 => "🌧️", // Chuva
                71 or 73 or 75 or 77 => "❄️", // Neve
                80 or 81 or 82 => "⛈️", // Pancadas de chuva
                95 or 96 or 99 => "🌩️", // Trovoada
                _ => "☁️" // Padrão
            };
        }
        private readonly HttpClient _httpClient;

        // Injetando o HttpClient
        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Ação para carregar a página inicial vazia
        [HttpGet]
        public IActionResult Index()
        {
            return View(new PrevisaoViewModel());
        }

        // Ação acionada ao clicar no botão de busca
        [HttpPost]
        public async Task<IActionResult> Index(string cidade)
        {
            var viewModel = new PrevisaoViewModel { CidadePesquisada = cidade };

            if (string.IsNullOrWhiteSpace(cidade))
            {
                viewModel.MensagemErro = "Por favor, digite o nome de uma cidade.";
                return View(viewModel);
            }

            try
            {
                // 1. Buscar a Latitude e Longitude da cidade
                var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={cidade}&count=1";
                var geoResponse = await _httpClient.GetStreamAsync(geoUrl);
                var geoData = await JsonSerializer.DeserializeAsync<GeoCodificacaoResposta>(geoResponse);

                if (geoData?.Resultados == null || geoData.Resultados.Count == 0)
                {
                    viewModel.MensagemErro = "Cidade não encontrada.";
                    return View(viewModel);
                }

                var lat = geoData.Resultados[0].Latitude;
                var lon = geoData.Resultados[0].Longitude;

                // 1. Converte para texto forçando o uso do PONTO em vez da vírgula
                string latFormatada = lat.ToString(CultureInfo.InvariantCulture);
                string lonFormatada = lon.ToString(CultureInfo.InvariantCulture);

                // 2. Usa as variáveis formatadas na URL
                // Use diretamente a URL que contém o weather_code
                var climaUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latFormatada}&longitude={lonFormatada}&daily=temperature_2m_max,temperature_2m_min,weather_code&forecast_days=10&timezone=auto";
                var climaResposta = await _httpClient.GetStreamAsync(climaUrl);
                var climaData = await JsonSerializer.DeserializeAsync<ClimaApiResposta>(climaResposta);

                // 3. Mapear os dados da API para o nosso ViewModel
                if (climaData?.Diario != null)
                {
                    for (int i = 0; i < climaData.Diario.Tempo.Count; i++)
                    {
                        viewModel.Previsoes.Add(new DiaPrevisao
                        {
                            Data = climaData.Diario.Tempo[i],
                            TempMaxima = climaData.Diario.TemperaturaMaxima[i],
                            TempMinima = climaData.Diario.TemperaturaMinima[i],
                            IconeAnimado = ObterIconeClima(climaData.Diario.CodigoClima[i])
                        });
                    }
                }
            }
                catch (Exception ex)
            {
                viewModel.MensagemErro = $"Erro detalhado: {ex.Message}";
            }

            // Retorna a View preenchida com os dados
            return View(viewModel);

        }
    }
}
