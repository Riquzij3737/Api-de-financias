using FacinorasContaminadospeloodio.ManiPulationJson;
using Microsoft.AspNetCore.Mvc;

namespace FacinorasContaminadospeloodio.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UsersController
{
    public EntityFrameworkMalFeitoUsers MalFeito = new EntityFrameworkMalFeitoUsers();
    
    [HttpPost("/AddUser")]
    public async Task<IResult> Post([FromQuery]string nome, [FromQuery]string email, [FromQuery]string password, [FromQuery]int cpf)
    {
        return await MalFeito.Create(nome, email, password, cpf);
    }

    [HttpDelete("/DeleteUser/{Cpf}")]
    public async Task<IResult> Delete([FromRoute] int Cpf)
    {
        return await MalFeito.Delete(Cpf);
    }

    [HttpPut("/UpdateUser")]
    public async Task<IResult> Update([FromQuery] string nome, [FromQuery] string email, [FromQuery] string password,[FromQuery]int cpf)
    {
        return await MalFeito.Update(nome, email, password, cpf);
    }

    
    [HttpGet("/GetUser/{key}")]
    public async Task<IResult> Get([FromRoute] string key)
    {
        return await MalFeito.Reader(key);
    }
}