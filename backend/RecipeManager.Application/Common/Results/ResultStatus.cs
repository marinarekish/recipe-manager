namespace RecipeManager.Application.Common.Results;

public enum ResultStatus
{
    Ok,
    NotFound,
    NoContent,
    ValidationError,
    Conflict,
    Forbidden,
    Unauthorized
}