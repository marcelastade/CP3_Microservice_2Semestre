using CP3.Domain;
using CP3.Repository;
using CP3.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CP3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private const string cacheKey = "usuarios-cache";
        private readonly ICacheService cacheService;
        private readonly ILogger<UsuarioController> logger;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository, ICacheService cacheService, ILogger<UsuarioController> logger)
        {
            this._usuarioRepository = usuarioRepository;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> AddUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
                return BadRequest("Usuário inválido.");

            try
            {
                usuario.DataCadastro = DateTime.Now;
                int id = await _usuarioRepository.AddUsuarioAsync(usuario);
                return Ok(new { Message = "Usuário cadastrado com sucesso!", Id = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao cadastrar usuário: {ex.Message}");
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
