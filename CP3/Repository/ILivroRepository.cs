using CP3.Domain;

namespace CP3.Repository
{
    public interface ILivroRepository
    {
        Task<Livro?> GetLivroByIdAsync(int isbn);
        Task<int> AddLivroAsync(Livro livro);
        Task AtualizarStatusAsync(int isbn, string novoStatus);
    }
}
