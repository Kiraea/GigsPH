using Microsoft.AspNetCore.Mvc;
using PrjA.Common;

namespace PrjA.Infrastructure;
// stsatic cause if we do class only we have to create an obj first which is meh compared to just direct call
public static class ResultHttpExtensions
{
    // static allows you to just calls the (thisvar).(method) without needing the static class name
    // but extnesion methods only work if parent is static class
    public static ObjectResult ToProblemDetails<T>(this Result<T> result, ControllerBase controller)
    {
        var error = result.Error;

        if (error == null)
        {
            throw new InvalidOperationException("Result needs an error if failed");
            
        }

        int statusCode = error.Type switch
        {
            ErrorType.BadRequest => 400,
            ErrorType.Forbidden => 403,
            ErrorType.NotFound => 404,
            ErrorType.Unauthorized => 401,
            _ => 500
        };
        return controller.Problem(statusCode:  statusCode, title: error.Code, detail: error.Message);
        
    }
}
