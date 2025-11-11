using Dapper;
using CP3.Domain;
using MySqlConnector;

namespace CP3.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly MySqlConnection _connection;
        public UsuarioRepository(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        public async Task<int> AddUsuarioAsync(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario), "Usuário inválido.");
            await _connection.OpenAsync();
            string sql = @"
                INSERT INTO Usuario (IdUsuario, Nome, Email, Tipo, DataCadastro)
                VALUES (@IdUsuario, @Nome, @Email, @Tipo, @DataCadastro);
                SELECT LAST_INSERT_ID();
            ";
            var id = await _connection.ExecuteScalarAsync<int>(sql, usuario);
            await _connection.CloseAsync();
            return id;
        }
    }
}
