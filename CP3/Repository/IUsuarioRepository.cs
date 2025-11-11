using CP3.Domain;

namespace CP3.Repository
{
    public interface IUsuarioRepository
    {
        Task<int> AddUsuarioAsync(Usuario usuario);
    }
}
