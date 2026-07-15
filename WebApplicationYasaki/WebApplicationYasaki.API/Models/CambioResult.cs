using System.Text.Json.Serialization;

namespace WebApplicationYasaki.API.Models
{
    // A AwesomeAPI retorna chaves dinâmicas baseadas nas moedas consultadas (ex: "USDEUR").
    // Herdando de Dictionary<string, InfoCambio>, o .NET mapeia dinamicamente qualquer par.
    public class AwesomeApiResponse : Dictionary<string, InfoCambio>
    {
    }

    public class InfoCambio
    {
        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("codein")]
        public string codein { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("high")]
        public string high { get; set; }

        [JsonPropertyName("low")]
        public string low { get; set; }

        [JsonPropertyName("varBid")]
        public string varBid { get; set; }

        [JsonPropertyName("pctChange")]
        public string pctChange { get; set; }

        [JsonPropertyName("bid")]
        public string bid { get; set; }

        [JsonPropertyName("ask")]
        public string ask { get; set; }

        [JsonPropertyName("timestamp")]
        public string timestamp { get; set; }

        [JsonPropertyName("create_date")]
        public string create_date { get; set; }
    }
}