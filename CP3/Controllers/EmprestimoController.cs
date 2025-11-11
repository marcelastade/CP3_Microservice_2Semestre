using CP3.Domain;
using CP3.Repository;
using CP3.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CP3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmprestimoController : ControllerBase
    {
        private const string cacheKey = "emprestimos-cache";
        private readonly IEmprestimoRepository _emprestimoRepository;
        private readonly ICacheService cacheService;
        private readonly ILogger<EmprestimoController> logger;

        public EmprestimoController(IEmprestimoRepository emprestimoRepository, ICacheService cacheService, ILogger<EmprestimoController> logger)
        {
            this._emprestimoRepository = emprestimoRepository;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        [HttpPost("realizar")]
        public async Task<IActionResult> AddEmprestimo([FromBody] Emprestimo emprestimo)
        {
            if (emprestimo == null)
                return BadRequest("Empréstimo inválido.");

            try
            {
                emprestimo.DataEmprestimo = DateTime.Now;
                int id = await _emprestimoRepository.AddEmprestimoAsync(emprestimo);
                return Ok(new { Message = "Empréstimo realizado com sucesso!", Id = id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao realizar empréstimo: {ex.Message}");
            }
        }

        [HttpGet("{idUsuario}/ativos")]
        public async Task<IActionResult> GetEmprestimosAtivos(int idUsuario)
        {
            try
            {
                int total = await _emprestimoRepository.GetEmprestimosAsync(idUsuario);
                return Ok(new { IdUsuario = idUsuario, EmprestimosAtivos = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao consultar empréstimos: {ex.Message}");
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

