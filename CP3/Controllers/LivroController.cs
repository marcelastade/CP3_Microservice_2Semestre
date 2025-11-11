using CP3.Domain;
using CP3.Repository;
using CP3.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CP3.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LivroController : ControllerBase
    {
        private const string cacheKey = "livros-cache";
        private readonly ILivroRepository _livroRepository;
        private readonly ICacheService cacheService;
        private readonly ILogger<LivroController> logger;

        public LivroController(ILivroRepository livroRepository, ICacheService cacheService, ILogger<LivroController> logger)
        {
            this._livroRepository = livroRepository;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> AddLivro([FromBody] Livro livro)
        {
            if (livro == null)
                return BadRequest("Livro inválido.");

            try
            {
                livro.DataCadastro = DateTime.Now;
                livro.Status = "DISPONIVEL";
                int id = await _livroRepository.AddLivroAsync(livro);
                return Ok(new { Message = "Livro cadastrado com sucesso!", Id = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao cadastrar livro: {ex.Message}");
            }
        }

        [HttpGet("{isbn}")]
        public async Task<IActionResult> GetLivro(int isbn)
        {
            try
            {
                var livro = await _livroRepository.GetLivroByIdAsync(isbn);
                if (livro == null)
                    return NotFound("Livro não encontrado.");

                return Ok(livro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar livro: {ex.Message}");
            }
        }

        [HttpPut("{isbn}/status")]
        public async Task<IActionResult> AtualizarStatus(int isbn, [FromQuery] string novoStatus)
        {
            try
            {
                await _livroRepository.AtualizarStatusAsync(isbn, novoStatus);
                return Ok(new { Message = $"Status do livro {isbn} atualizado para {novoStatus}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar status do livro: {ex.Message}");
            }
        }

        private async Task InvalidateCache()
        {
            try
            {
                await cacheService.DeleteAsync(cacheKey);
                logger.LogInformation("Cache invalidado com sucesso");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Erro ao invalidar cache Redis, mas operação continuará");
            }
        }
    }
    
}
