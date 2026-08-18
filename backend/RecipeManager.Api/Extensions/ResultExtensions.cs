using Microsoft.AspNetCore.Mvc;
using RecipeManager.Application.Common.Results;

namespace RecipeManager.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkResult(),
            ResultStatus.NotFound => result.ErrorMessage is not null
                ? new NotFoundObjectResult(new { message = result.ErrorMessage })
                : new NotFoundResult(),
            ResultStatus.ValidationError => new BadRequestObjectResult(
                new
                {
                    message = result.ErrorMessage ?? "Validation failed",
                    errors = result.Errors
                }),
            ResultStatus.Conflict => new ConflictObjectResult(
                new { message = result.ErrorMessage }),
            ResultStatus.Forbidden => new ForbidResult(),
            ResultStatus.Unauthorized => new UnauthorizedResult(),
            _ => new StatusCodeResult(500)
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.Status == ResultStatus.Ok)
            return new OkObjectResult(result.Value);

        return ((Result)result).ToActionResult();
    }
}
