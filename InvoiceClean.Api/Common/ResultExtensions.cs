using InvoiceClean.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceClean.Api.Common;

public static class ResultExtensions
{
    public static ActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess) return controller.Ok(result.Value);
        return controller.ProblemFromError(result.Error!);
    }

    public static ActionResult CreatedFromResult(this ControllerBase controller, string actionName, object routeValues, Result<Guid> result)
    {
        if (result.IsSuccess)
            return controller.CreatedAtAction(actionName, routeValues, result.Value);

        return controller.ProblemFromError(result.Error!);
    }

    private static ActionResult ProblemFromError(this ControllerBase controller, Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        if (error.Type == ErrorType.Validation && error.ValidationErrors is not null)
        {
            var vpd = new ValidationProblemDetails
            {
                Title = error.Message,
                Status = status
            };

            foreach (var kv in error.ValidationErrors)
                vpd.Errors.Add(kv.Key, kv.Value);

            return controller.BadRequest(vpd);
        }

        return controller.Problem(
            title: error.Message,
            detail: error.Code,
            statusCode: status
        );
    }
}
