// Importando namespaces nescessarios para a criação do codigo

using System.Diagnostics.CodeAnalysis;
using FacinorasContaminadospeloodio.ManiPulationJson;
using FacinorasContaminadospeloodio.Model;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Newtonsoft.Json;

namespace FacinorasContaminadospeloodio.DataQuerys;

// definindo a classe contendo os metodos relevante as funções de transações
public class TransacoesMalFeito : EntityFrameworkMalFeitoUsers
{
    // string da chave de conexão
    public readonly string ConnectionString =
        "Server=localhost;Database=mubank_db;Uid=root;Pwd=Z7$J8v2mL@pQxW4tY#RkN9f6S!bG5A;";

    // Metodo principal para enviar transações e salvar o registro delas no banco de dados
    public async Task<IResult> SalvarEenviar(string titulo, string descricao, int cpf, float valor,
        DateTime datadeenvio)
    {
        // usa o metodo verificarseénulo() para analisar se os valores inseridos são nulos
        if (verificarseénulo(titulo, descricao, cpf, valor, datadeenvio))
        {
            return Results.NoContent(); // caso sejam, retorna a resposta http 204 
        } // caso contrario

        // abre a conexão com o banco de dados
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync(); 

            // cria um novo comando para o servidor MySql
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                // o comando? uma query do sql para adicionar as infomações no banco de dados
                cmd.CommandText =
                    "INSERT INTO transacoes(Titulo, Descricao, Preco, CPF, Data_envio) VALUES (@Titulo, @Descricao, @Preco, @CPF, @Data_envio);";

                // abro um bloco try catch para 
                try
                {
                    cmd.Parameters.Clear();

                    // informar os parametros contido na queyr
                    cmd.Parameters.AddWithValue("@Titulo", titulo);
                    cmd.Parameters.AddWithValue("@Descricao", descricao);
                    cmd.Parameters.AddWithValue("@Preco", valor);
                    cmd.Parameters.AddWithValue("@CPF", cpf);
                    cmd.Parameters.AddWithValue("@Data_envio", datadeenvio);

                    // executar dps a query de formar assincrona
                    await cmd.ExecuteNonQueryAsync();
                    
                    // dps incrimento o valor de dinheiro contido na transação na conta do usuario
                    // que contenha um cpf equivalente ao cpf inserido na hora de informar
                    // a transação
                    cmd.CommandText = "UPDATE users_tb SET Dinheiro=Dinheiro + @Dinheiro  WHERE CPF=@CPF";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CPF", cpf);
                    cmd.Parameters.AddWithValue("@Dinheiro", valor);
                    
                    // dps executo a query
                    await cmd.ExecuteNonQueryAsync();

                    return Results.Ok(); // e retorno um codigo http 200 OK
                }
                catch (Exception e) // caso haja um erro
                {
                    return Results.BadRequest(e.Message); // retorno um badrequest

                }
            }
        }

    }

    // metodo verificarseénulo, mesmo usado no metodo para enviar as transações
    public bool verificarseénulo(params object[] valores)
    {
        // itero sobre o array de objetos contido no params de entrada do metodo
        foreach (var VARIABLE in valores)
        {
            // verifico se VARIABLE é nulo
            if (VARIABLE is null)
            {
                return true; // se for, retorno true
            }
            else // caso contrario
            {
                Console.WriteLine($"{VARIABLE} não é nulo"); // ent, n sabia oq fazer aq, ent só coloquei isso mnesmo
            }
        }

        return false; // caso a iteração acabe, e n tenha retornado true nenhum dos membros da coleção do array de objetos, retorno false, constando que todos os parametros inseridos tem ou contem algum valor
    }

    
    // metodo de limpar o historico
    public async Task<IResult> LimparHistorico()
    {
        // começo abrindo uma conexão com o banco de dados usando a string de conexão que esta definida como global
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync(); // abro de forma assincrona, pq? pq da mais desempenho

            // Crio uma instancia do MySqlcommand para realizar comandos sql dentro do servidor
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM transacoes"; // crio uma query insegura para limpar o banco de dados, SEMPRE QUE VOCê USAR O DELETE COLOQUE UM WHERE, A N SER SE VC QUER APAGAR TODOS OS DADOOS DO BANCO DE DADOS!!!!!!

                try // Crio um bloco try catch
                {
                    await cmd.ExecuteNonQueryAsync(); // executo a query de forma assincrona, pq? motivos de desempenho

                    return Results.Ok("Limpando transacoes, Limpado Com sucesso!"); // e retorno um codigo de status 200 dizendo que a requisição foi bem sucessidida
                }
                catch (Exception e) // caso haja um erro
                {
                    return Results.BadRequest(e.Message); // retorno um codigo de status 400 dizendo que houve um erro na requisição
                }
            }
        }
    }

    // metodo para retornar todas as transações feitas
    public async Task<IResult> PegarHistorico(bool filtro = false) // esse parametro server para retornar algumas estatistiscas sobre as transações feitas na ultima hora
    {
        // abro uma conexão com o banco de dados
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            await conn.OpenAsync(); // de novo, de forma assincrona, pq? Motivos de desempenho:)

            using (MySqlCommand cmd = conn.CreateCommand()) // crio uma instancia do mysql command para executar comandos sql dentro do servidor 
            {
                cmd.CommandText = "SELECT * FROM transacoes"; // a query para pegar todas as informações

                using (MySqlDataReader reader = cmd.ExecuteReader()) // crio um reader para coletar os dados do banco de dados
                {
                    List<TransationsModel> models = new List<TransationsModel>(); // crio uma List de models para pegar todas as transações feitas, assim, eu pego todas, e n uma só

                    // abro um bloco try catch
                    try
                    {
                        // inicio um while,
                        while (reader.Read()) // enquanto houver linhas de informações dentro do banco de dados
                        {
                            // defino as prorpiedades de model, com os dados coletados do banco de dados
                            TransationsModel model = new TransationsModel
                            {
                                Titulo = reader.GetString("Titulo"),
                                Descricao = reader.GetString("Descricao"),
                                cpf = reader.GetInt32("CPF"),
                                Valor = reader.GetFloat("Preco"),
                                Datadeenvio = reader.GetDateTime("Data_envio")
                            };

                            // e adiciono a model dentro da lista
                            models.Add(model);
                        }

                        // verifico se filtro foi ativado
                        if (filtro)
                        {
                            // caso ele tenha sido ativado
                            /*
                             * filtro = true
                             */
                            // Verifica se models não é nulo ou vazio antes de calcular as estatísticas
                            if (models.Any())
                            {
                                // crio uma model para as estatiscas das ultimas transações feita na ultima hora
                                StaticsModel smodell = new StaticsModel
                                {
                                    count = models.Count,
                                    max = models.Max(x => x.Valor), // Maior valor
                                    min = models.Min(x => x.Valor), // Menor valor
                                    sum = models.Sum(x => x.Valor), // Soma de todos os valores
                                    avg = models.Average(x => x.Valor) // Média dos valores
                                };

                                return Results.Ok(smodell); // dps de realizar os calculos usando o LINQ, retorno eles em formato json
                            }
                            else // caso models seja nulo
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

                                return Results.Ok(smodell); // tmb retorno em formato json, só que tudo zero
                            }
                        }
                        else
                        {
                            return Results.Ok(models); // e caso filtro esteja desativado, retorno apena a lista de transações feitas sem aplicar as estatiscas
                        }
                    }
                    catch (Exception e) // e caso haja qualquer erro
                    {
                        return Results.BadRequest(e.Message); // retorno um codigo de status 400 dizendo que houve um erro de requisição
                    }
                }
            }
        }
    }
}
