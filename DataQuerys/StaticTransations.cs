using FacinorasContaminadospeloodio.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FacinorasContaminadospeloodio.DataQuerys;

public class StaticTransations : TransacoesMalFeito
{
    public async Task<IResult> CalcularEstatisticasAsync()
    {
        try
        {
            // Obtém o histórico como string (ou de outra forma) e converte para array de objetos JSON
             var jsonString = await PegarHistorico(true);
            JArray array = JArray.Parse(jsonString.ToString().Remove(1));

            // Usando LINQ para aplicar os cálculos de forma mais eficiente
            var precos = array
                .Where(x => x["Preco"] != null)  // Filtro para garantir que o campo "Preco" existe
                .Select(x => x["Preco"].Value<double>())  // Seleciona os preços
                .ToList();

            // Caso a lista esteja vazia
            if (!precos.Any())
            {
                return Results.BadRequest("Nenhum preço encontrado.");
            }

            // Cria o objeto de estatísticas
            StaticsModel estatisticas = new StaticsModel
            {
                count = precos.Count,
                sum = precos.Sum(),
                avg = precos.Average(),
                min = precos.Min(x => array["Preco"].Value<int>()),
                max = precos.Max(x => array["Preco"].Value<int>())
            };

            // Retorna os resultados filtrados e as estatísticas
            return Results.Ok(estatisticas);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }
}