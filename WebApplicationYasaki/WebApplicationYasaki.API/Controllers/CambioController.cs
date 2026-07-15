using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace WebApplicationYasaki.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CambioController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        public CambioController(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        [HttpGet("converter")]
        public async Task<IActionResult> ObterCambio([FromQuery] string de, [FromQuery] string para)
        {
            if (string.IsNullOrEmpty(de) || string.IsNullOrEmpty(para))
            {
                return BadRequest("Os parâmetros 'de' e 'para' são obrigatórios.");
            }

            de = de.ToUpper().Trim();
            para = para.ToUpper().Trim();

            string cacheKey = $"cambio_{de}_{para}";

            // 1. Tenta recuperar da cache para evitar o erro 429 (Muitas requisições)
            if (_cache.TryGetValue(cacheKey, out object? resultadoGuardado) && resultadoGuardado is not null)
            {
                return Ok(resultadoGuardado);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                string url;
                bool inverterTaxa = false;

                if (de == "BRL")
                {
                    url = $"https://economia.awesomeapi.com.br/last/{para}-{de}";
                    inverterTaxa = true;
                }
                else
                {
                    url = $"https://economia.awesomeapi.com.br/last/{de}-{para}";
                }

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var erroApi = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Erro na API externa: {erroApi}");
                }

                // Lemos a resposta diretamente como Dicionários Genéricos do C#!
                // Isso descarta a necessidade de QUALQUER classe auxiliar (como InfoCambioRapido).
                var dados = await response.Content.ReadFromJsonAsync<Dictionary<string, Dictionary<string, string>>>();
                string chave = inverterTaxa ? $"{para}{de}" : $"{de}{para}";

                if (dados != null && dados.TryGetValue(chave, out var info))
                {
                    // Lemos os textos do dicionário usando as chaves em minúsculo da AwesomeAPI
                    decimal taxa = Convert.ToDecimal(info["bid"], System.Globalization.CultureInfo.InvariantCulture);
                    decimal maximo = Convert.ToDecimal(info["high"], System.Globalization.CultureInfo.InvariantCulture);
                    decimal minimo = Convert.ToDecimal(info["low"], System.Globalization.CultureInfo.InvariantCulture);
                    string nomeOriginal = info["name"];

                    if (inverterTaxa)
                    {
                        taxa = 1 / taxa;
                        decimal tempMax = maximo;
                        maximo = 1 / minimo;
                        minimo = 1 / tempMax;
                    }

                    var respostaSucesso = new
                    {
                        Par = $"{de}/{para}",
                        Nome = inverterTaxa ? $"Real Brasileiro/{nomeOriginal.Split('/')[0]}" : nomeOriginal,
                        TaxaAtual = taxa,
                        UltimoMaximo = maximo,
                        UltimoMinimo = minimo,
                        DataConsulta = DateTime.Now
                    };

                    // Salva o resultado na cache por 3 minutos
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                    _cache.Set(cacheKey, respostaSucesso, cacheOptions);

                    return Ok(respostaSucesso);
                }

                return NotFound("Não foi possível processar as informações do par solicitado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }
    }
}