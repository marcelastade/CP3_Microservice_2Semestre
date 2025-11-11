using CP3.Domain;

namespace CP3.Repository
{
    public interface IEmprestimoRepository
    {
        Task<int> GetEmprestimosAsync(int id);
        Task<int> AddEmprestimoAsync(Emprestimo emprestimo);

    }
}
