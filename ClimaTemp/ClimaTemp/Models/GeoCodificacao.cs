using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ClimaTemp.Models
{
    public class GeoCodificacaoResposta
    {
        // O nome do JSON da API é "results"
        [JsonPropertyName("results")]
        public List<Localizacao> Resultados { get; set; }
    }

    public class Localizacao
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public class ClimaApiResposta
    {
        // O nome do JSON da API é "daily"
        [JsonPropertyName("daily")]
        public DadosDiario Diario { get; set; }
    }

    public class DadosDiario
    {
        // O nome do JSON da API é "time"
        [JsonPropertyName("time")]
        public List<string> Tempo { get; set; }

        // O nome do JSON da API é "temperature_2m_max"
        [JsonPropertyName("temperature_2m_max")]
        public List<double> TemperaturaMaxima { get; set; }

        // O nome do JSON da API é "temperature_2m_min"
        [JsonPropertyName("temperature_2m_min")]
        public List<double> TemperaturaMinima { get; set; }

        // Json para receber o código do clima
        [JsonPropertyName("weather_code")]
        public List<int> CodigoClima { get; set; }

    }

    public class PrevisaoViewModel
    {
        public string CidadePesquisada { get; set; }
        public List<DiaPrevisao> Previsoes { get; set; } = new List<DiaPrevisao>();
        public string MensagemErro { get; set; }
    }

    public class DiaPrevisao
    {
        public string Data { get; set; }

        // Corrigido de string para double para não dar erro no Controller
        public double TempMaxima { get; set; }
        public double TempMinima { get; set; }
        public string IconeAnimado { get; internal set; }
    }
}