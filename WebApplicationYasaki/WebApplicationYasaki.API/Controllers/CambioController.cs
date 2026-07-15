using Microsoft.AspNetCore.Mvc;
using WebApplicationYasaki.API.Models;

namespace WebApplicationYasaki.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")] // Rota de acesso: /api/cambio
    public class CambioController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Construtor que recebe o criador de clientes HTTP do .NET
        public CambioController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// GET: api/cambio/converter?de=USD&para=EUR
        /// </summary>
        [HttpGet("converter")]
        public async Task<IActionResult> ObterCambio(string de = "USD", string para = "EUR")
        {
            // Normaliza para maiúsculas (ex: usd -> USD)
            de = de.ToUpper();
            para = para.ToUpper();

            try
            {
                // Criamos o cliente HTTP para fazer o pedido externo
                var client = _httpClientFactory.CreateClient();

                // URL da AwesomeAPI para consultar o par de moedas desejado
                string url = $"https://economia.awesomeapi.com.br/last/{de}-{para}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest($"Não foi possível obter a conversão para o par {de}-{para}. Verifique se as siglas estão corretas.");
                }

                // Deserializa o JSON recebido para a nossa estrutura C#
                var dados = await response.Content.ReadFromJsonAsync<AwesomeApiResponse>();

                string chave = $"{de}{para}"; // Ex: "USDEUR"
                if (dados != null && dados.TryGetValue(chave, out var infoCambio))
                {
                    // Devolvemos um JSON limpo e bem estruturado para o nosso front-end
                    return Ok(new
                    {
                        Par = $"{de}/{para}",
                        Nome = infoCambio.name,
                        TaxaAtual = Convert.ToDecimal(infoCambio.bid, System.Globalization.CultureInfo.InvariantCulture),
                        UltimoMaximo = Convert.ToDecimal(infoCambio.high, System.Globalization.CultureInfo.InvariantCulture),
                        UltimoMinimo = Convert.ToDecimal(infoCambio.low, System.Globalization.CultureInfo.InvariantCulture),
                        DataConsulta = DateTime.Now
                    });
                }

                return NotFound("Dados de câmbio não encontrados no retorno.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao processar conversão: {ex.Message}");
            }
        }
    }
}