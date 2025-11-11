using CP3.Domain;
using Dapper;
using MySqlConnector;

namespace CP3.Repository
{
    public class EmprestimoRepository : IEmprestimoRepository
    {
        private readonly MySqlConnection _connection;
        private readonly LivroRepository _livroRepository;

        public EmprestimoRepository(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _livroRepository = new LivroRepository(connectionString);
        }

        public async Task<int> GetEmprestimosAsync(int id)
        {
            if (_connection.State != System.Data.ConnectionState.Open)
                await _connection.OpenAsync();
            string sql = @"
                SELECT COUNT(*) FROM Emprestimo WHERE IdUsuario = @id AND Status = 'ATIVO';                
            ";
            var total = await _connection.ExecuteScalarAsync<int>(sql, new { id });
            await _connection.CloseAsync();
            return total;
        }

        public async Task<int> AddEmprestimoAsync(Emprestimo emprestimo)
        {
            if (emprestimo == null)
                throw new ArgumentNullException(nameof(emprestimo), "Empréstimo inválido.");

            var emprestimoUsuario = await GetEmprestimosAsync(emprestimo.IdUsuario);

            if (emprestimoUsuario >=3)
                throw new ArgumentNullException(nameof(emprestimo), "Não é permitido empréstimo de livros porque o usuário já tem 3 empréstimos ativos.");

            var livro = await _livroRepository.GetLivroByIdAsync(emprestimo.ISBN);
            if (livro.Status != "DISPONIVEL")
                throw new InvalidOperationException("Livro não está disponível para empréstimo.");

            if (livro == null)
                throw new InvalidOperationException("Livro não encontrado.");

            if (_connection.State != System.Data.ConnectionState.Open)
                await _connection.OpenAsync();
            string sql = @"
                INSERT INTO Emprestimo (IdEmprestimo, ISBN, IdUsuario, DataEmprestimo, DataPrevistaDev, DataRealDev, Status)
                VALUES (@IdEmprestimo, @ISBN, @IdUsuario, @DataEmprestimo, @DataPrevistaDev, @DataRealDev, @Status);
                SELECT LAST_INSERT_ID();
            ";
            var id = await _connection.ExecuteScalarAsync<int>(sql, emprestimo);
            await _livroRepository.AtualizarStatusAsync(emprestimo.ISBN, "EMPRESTADO");
            await _connection.CloseAsync();
            return id;
        }
    }
}
