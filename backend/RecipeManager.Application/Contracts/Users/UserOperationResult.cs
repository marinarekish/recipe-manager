namespace RecipeManager.Application.Contracts.Users;

public enum UserOperationStatus
{
    Ok,
    NotFound
}

public record UserUpdateResult(
    UserOperationStatus Status,
    UserResponse? User
);
