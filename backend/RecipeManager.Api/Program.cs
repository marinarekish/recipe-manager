using RecipeManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Interfaces;
using RecipeManager.Application.Mapping;
using RecipeManager.Application.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ILoginCodeService, LoginCodeService>();
        
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddScoped<IRecipeService, RecipeService>();
        builder.Services.AddScoped<IFavoriteService, FavoriteService>();
        
        builder.Services.AddScoped<IIngredientService, IngredientService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICuisineService, CuisineService>();
        
        builder.Services.AddAutoMapper(cfg => { }, typeof(RecipeMappingProfile).Assembly);
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}