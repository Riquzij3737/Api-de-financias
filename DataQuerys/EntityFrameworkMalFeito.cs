using System.Data;
using System.Text;
using FacinorasContaminadospeloodio.Model;
using FacinorasContaminadospeloodio.Security;
using MySql.Data.MySqlClient;

namespace FacinorasContaminadospeloodio.ManiPulationJson;

public class EntityFrameworkMalFeitoUsers
{
    public readonly string ConnectionString = "Server=localhost;Database=mubank_db;Uid=root;Pwd=Z7$J8v2mL@pQxW4tY#RkN9f6S!bG5A;";
    
    public async Task<IResult> Create(string nome, string Email, string senha, int CPF)
    {
        Encrypt crypt = new Encrypt();

        string senhacriptografada = await crypt.EncryptTextAsync(senha);
        
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO users_tb(Nome, Email, Senha, CPF) VALUES(@Nome, @Email, @Senha, @CPF);";

                try
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Senha", senhacriptografada);
                    cmd.Parameters.AddWithValue("@CPF", CPF);

                    await cmd.ExecuteNonQueryAsync();

                    return Results.Ok("deu certo paizão");
                }
                catch (MySqlException sqlex)
                {
                    return Results.InternalServerError($"Deu erro aq no server adm, n se preocupa n paizao, aq O erro: {sqlex.Message}");
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }
        }
            
    }

    public async Task<IResult> Update(string nome, string email, string senha, int CPF)
    {
        Encrypt crypt = new Encrypt();

        string senhacriptografada = await crypt.EncryptTextAsync(senha);

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "UPDATE users_tb SET Nome=@Nome, Senha=@Senha, Email=@Email WHERE CPF=@CPF;";

                try
                {
                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@Senha", senhacriptografada);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@CPF", CPF);

                    await cmd.ExecuteNonQueryAsync();

                    return Results.Ok("Banco de dados atualizado com sucesso");
                }
                catch (MySqlException sqlex)
                {
                    return Results.InternalServerError("erro aq dentro, perai");
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }
        }
    }

    public async Task<IResult> Delete(int CPF)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = "DELETE FROM users_tb WHERE CPF=@CPF;";
                    cmd.Parameters.AddWithValue("@CPF", CPF);
                    
                    await cmd.ExecuteNonQueryAsync();
                    
                    return Results.Ok("Deletado com sucesso");
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }
        }
    }

    public async Task<IResult> Reader(string Key)
    {
        if (Key != "ChaveMuitoSegura")
        {
            return Results.BadRequest("Não inseriu a chave correta :D");
        }

        List<UsersModel> users = new List<UsersModel>();

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM users_tb";

                using (MySqlDataReader reader = cmd.ExecuteReader())  // Correção: ExecuteReaderAsync
                {
                    Descrypto descrypto = new Descrypto();

                    int i = 0;
                    
                    while (await reader.ReadAsync())  // Correção: ReadAsync
                    {
                        string senhasalva = reader["Senha"] as string;  // Pega os dados binários diretamente

                        if (senhasalva != null)
                        {
                            UsersModel model = new UsersModel();  // Criar um novo objeto para cada iteração
                            model.cpf = reader.GetInt32("CPF");
                            model.Email = reader.GetString("Email");
                            model.Name = reader.GetString("Nome");
                            model.Password = await descrypto.Decrypt(senhasalva);  // Decripta os dados binários

                            users.Add(model);  // Adiciona o modelo à lista
                        }
                    }
                }
            }
        }

        if (users.Count == 0)
        {
            return Results.NotFound("Nenhum usuário encontrado.");
        }

        return Results.Ok(users);  // Retorna a lista de usuários encontrados
    }

}