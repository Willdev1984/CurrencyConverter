using System.Collections;

namespace WebApplicationYasaki.API.Models
{
    public class CambioResult
    {
        // Esta classe vai mapear a resposta JSON da AwesomeAPI para USD-EUR ou BRL-EUR, etc.
        public string code { get; set; } = string.Empty;
        public string codein { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string high { get; set; } = string.Empty;
        public string low { get; set; } = string.Empty;
        public string bid { get; set; } = string.Empty;

    }
    // A API externa devolve um dicionário onde a chave é o par de moedas (ex: "USDEUR")
    public class AwesomeApiResponse : Dictionary<String, CambioResult>
    { 
    }
        
}
