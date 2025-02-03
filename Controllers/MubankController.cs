using FacinorasContaminadospeloodio.DataQuerys;
using Microsoft.AspNetCore.Mvc;

namespace FacinorasContaminadospeloodio.Controllers;

[ApiController]
[Route("Api/[controller]/Transações")]
public class MubankController
{
    [HttpPost]
    public async Task<IResult> Enviar([FromQuery] string titulo, [FromQuery] string descricao, [FromQuery] int cpf, [FromQuery] float valor)
    {
        DateTime datadeenvio = DateTime.Now;
        TransacoesMalFeito feito = new TransacoesMalFeito();
        
        return await feito.SalvarEenviar(titulo, descricao, cpf, valor, datadeenvio);
    }

    [HttpGet]
    public async Task<IResult> Get()
    {
        TransacoesMalFeito feito = new TransacoesMalFeito();
        
        return await feito.PegarHistorico();
    }

    [HttpDelete]
    public async Task<IResult> ClearHystoric()
    {
        TransacoesMalFeito feito = new TransacoesMalFeito();
        
        return await feito.LimparHistorico();
    }

    [HttpGet("/Statics")]
    public async Task<IResult> CalsASyncStatic()
    {
        TransacoesMalFeito feito = new TransacoesMalFeito();

        return await feito.PegarHistorico(true);
    }
}