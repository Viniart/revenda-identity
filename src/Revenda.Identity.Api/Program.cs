using Revenda.Identity.Api.Extensions;
using Revenda.Identity.Api.Middlewares;
using Revenda.Identity.Application;
using Revenda.Identity.Infrastructure;
using Revenda.Identity.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddIdentityApplication()
    .AddIdentityInfrastructure(builder.Configuration);

builder.Services.AddIdentityAuthentication();
builder.Services.AddIdentitySwagger();
builder.Services.AddControllers();
builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres") ?? string.Empty, name: "postgres");

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Revenda - Identidade v1"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

if (builder.Configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<DatabaseBootstrapper>().RunAsync(CancellationToken.None);
}

await app.RunAsync();

public partial class Program;
