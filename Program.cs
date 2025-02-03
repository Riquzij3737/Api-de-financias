var builder = WebApplication.CreateBuilder(args);

// Adiciona o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configura o Swagger para ser exibido na rota /swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseAuthorization();

app.Run();