using Microsoft.OpenApi.Models;

namespace Revenda.Identity.Api.Extensions;

public static class SwaggerExtensions
{
    public const string SecuritySchemeId = "Bearer";

    public static IServiceCollection AddIdentitySwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Revenda - Identidade",
                Version = "v1",
                Description = "Cadastro e autenticação de compradores da plataforma de revenda de veículos."
            });

            options.OperationFilter<AuthenticationRequirementOperationFilter>();

            options.AddSecurityDefinition(SecuritySchemeId, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o token devolvido por POST /auth/login."
            });
        });

        return services;
    }
}
