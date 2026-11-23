
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PrjA.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{

    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _env; 
    private readonly IProblemDetailsService _problemDetailsService; 
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _env = env;
        _problemDetailsService = problemDetailsService;
    }


    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled Excception on {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        httpContext.Response.StatusCode = 500;
        var problemDetails = new ProblemDetails()
        {
            Detail = _env.IsDevelopment() ? exception.Message : "Unexpected Error Occured",
            Title = "Server.UnexpectedError",
            Status = StatusCodes.Status500InternalServerError,

        };
        
        //ProblemDetailsContext is never serialized to JSON—ASP.NET Core only
        //zserializes the context.ProblemDetails property inside it, so Exception stays strictly on the server. 
        await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
        return true;


    }
}