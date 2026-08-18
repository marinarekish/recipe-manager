namespace RecipeManager.Application.Common.Results;

public enum ResultStatus
{
    Ok,
    NotFound,
    ValidationError,
    Conflict,
    Forbidden,
    Unauthorized
}