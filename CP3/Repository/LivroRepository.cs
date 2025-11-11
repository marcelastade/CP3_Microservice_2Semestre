using Dapper;
using CP3.Domain;
using MySqlConnector;

namespace CP3.Repository

{
    public class LivroRepository : ILivroRepository
    {
        private readonly MySqlConnection _connection;
        public LivroRepository(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        public async Task<int> AddLivroAsync(Livro livro)
        {
            if (livro == null)
                throw new ArgumentNullException(nameof(livro), "Livro inválido.");
            await _connection.OpenAsync();
            string sql = @"
                INSERT INTO Livro (ISBN, Titulo, Autor, Categoria, Status, DataCadastro)
                VALUES (@ISBN, @Titulo, @Autor, @Categoria, @Status, @DataCadastro);
                SELECT LAST_INSERT_ID();
            ";
            var id = await _connection.ExecuteScalarAsync<int>(sql, livro);
            await _connection.CloseAsync();
            return id;
        }

        public async Task<Livro?> GetLivroByIdAsync(int isbn) 
        {
            await _connection.OpenAsync();

            string sql = "SELECT * FROM Livro WHERE ISBN = @isbn;";
            var livro = await _connection.QueryFirstOrDefaultAsync<Livro>(sql, new { isbn });
            await _connection.CloseAsync();
            return livro;
        }

        public async Task AtualizarStatusAsync(int isbn, string novoStatus)
        {
            await _connection.OpenAsync();

            string sql = "UPDATE Livro SET Status = @novoStatus WHERE ISBN = @isbn;";
            await _connection.ExecuteAsync(sql, new { isbn, novoStatus });
            await _connection.CloseAsync();
        }
    }
}
