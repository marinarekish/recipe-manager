namespace RecipeManager.Application.Contracts.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone
    // how to add default roleId for new Users
    
    // seeding
    // builder.HasData(
    //     new Role { RoleId = 1, Name = "Administrator" },
    //     new Role { RoleId = 2, Name = "User" }
    // );
    );