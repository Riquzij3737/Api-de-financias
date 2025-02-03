using System.Diagnostics.CodeAnalysis;
using FacinorasContaminadospeloodio.ManiPulationJson;
using FacinorasContaminadospeloodio.Model;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Newtonsoft.Json;

namespace FacinorasContaminadospeloodio.DataQuerys;

public class TransacoesMalFeito : EntityFrameworkMalFeitoUsers
{
    public readonly string ConnectionString =
        "Server=localhost;Database=mubank_db;Uid=root;Pwd=Z7$J8v2mL@pQxW4tY#RkN9f6S!bG5A;";

    public async Task<IResult> SalvarEenviar(string titulo, string descricao, int cpf, float valor,
        DateTime datadeenvio)
    {
        if (verificarseénulo(titulo, descricao, cpf, valor, datadeenvio))
        {
            return Results.NoContent();
        }

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "INSERT INTO transacoes(Titulo, Descricao, Preco, CPF, Data_envio) VALUES (@Titulo, @Descricao, @Preco, @CPF, @Data_envio);";

                try
                {
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@Titulo", titulo);
                    cmd.Parameters.AddWithValue("@Descricao", descricao);
                    cmd.Parameters.AddWithValue("@Preco", valor);
                    cmd.Parameters.AddWithValue("@CPF", cpf);
                    cmd.Parameters.AddWithValue("@Data_envio", datadeenvio);

                    await cmd.ExecuteNonQueryAsync();

                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);

                }
            }
        }

    }

    public bool verificarseénulo(params object[] valores)
    {
        foreach (var VARIABLE in valores)
        {
            if (VARIABLE is null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public async Task<IResult> LimparHistorico()
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM transacoes";

                try
                {
                    await cmd.ExecuteNonQueryAsync();

                    return Results.Ok("Limpando transacoes, Limpado Com sucesso!");
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }
        }
    }

    public async Task<IResult> PegarHistorico(bool filtro = false)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM transacoes";

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    List<TransationsModel> models = new List<TransationsModel>();

                    try
                    {
                        while (reader.Read())
                        {
                            TransationsModel model = new TransationsModel
                            {
                                Titulo = reader.GetString("Titulo"),
                                Descricao = reader.GetString("Descricao"),
                                cpf = reader.GetInt32("CPF"),
                                Valor = reader.GetFloat("Preco"),
                                Datadeenvio = reader.GetDateTime("Data_envio")
                            };

                            models.Add(model);
                        }

                        if (filtro)
                        {
                            // Verifica se models não é nulo ou vazio antes de calcular as estatísticas
                            if (models.Any())
                            {
                                StaticsModel smodell = new StaticsModel
                                {
                                    count = models.Count,
                                    max = models.Max(x => x.Valor), // Maior valor
                                    min = models.Min(x => x.Valor), // Menor valor
                                    sum = models.Sum(x => x.Valor), // Soma de todos os valores
                                    avg = models.Average(x => x.Valor) // Média dos valores
                                };

                                return Results.Ok(smodell);
                            }
                            else
                            {
                                // Caso models seja vazio, retornar valores padrão
                                StaticsModel smodell = new StaticsModel
                                {
                                    count = 0,
                                    max = 0,
                                    min = 0,
                                    sum = 0,
                                    avg = 0
                                };

                                return Results.Ok(smodell);
                            }
                        }
                        else
                        {
                            return Results.Ok(models);
                        }
                    }
                    catch (Exception e)
                    {
                        return Results.BadRequest(e.Message);
                    }
                }
            }
        }
    }

}