using System.Net;
using Microsoft.AspNetCore.Mvc;
using Revenda.Identity.Application.Exceptions;
using Revenda.Identity.Domain.Exceptions;

namespace Revenda.Identity.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var problem = Translate(exception);

            if (problem.Status >= (int)HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Falha não tratada em {Method} {Path}",
                    context.Request.Method, context.Request.Path);
            }

            context.Response.StatusCode = problem.Status!.Value;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static ProblemDetails Translate(Exception exception) => exception switch
    {
        DomainException domain => Build(HttpStatusCode.BadRequest, "Requisição inválida", domain.Message),
        DuplicateCustomerException duplicate => Build(HttpStatusCode.Conflict, "Cadastro duplicado", duplicate.Message),
        CustomerNotFoundException notFound => Build(HttpStatusCode.NotFound, "Recurso não encontrado", notFound.Message),
        InvalidCredentialsException credentials =>
            Build(HttpStatusCode.Unauthorized, "Não autorizado", credentials.Message),
        UnauthorizedAccessException unauthorized =>
            Build(HttpStatusCode.Unauthorized, "Não autorizado", unauthorized.Message),
        _ => Build(
            HttpStatusCode.InternalServerError,
            "Erro interno",
            "Não foi possível concluir a operação.")
    };

    private static ProblemDetails Build(HttpStatusCode status, string title, string detail) => new()
    {
        Status = (int)status,
        Title = title,
        Detail = detail
    };
}
