using AutoMapper;
using RecipeManager.Application.Contracts.Recipes;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Mapping;

public class RecipeMappingProfile : Profile
{
    public RecipeMappingProfile()
    {
        // Entity → Response
        CreateMap<Recipe, RecipeResponse>()
            .ForMember(
                d => d.CuisineName,
                opt => opt.MapFrom(s => s.Cuisine.Name))
            .ForMember(
                d => d.CategoryName,
                opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(
                d => d.AuthorName,
                opt => opt.MapFrom(s => s.Author.FirstName + " " + s.Author.LastName))
            .ForMember(
                d => d.Ingredients,
                opt => opt.MapFrom(s => s.RecipeIngredients));

        CreateMap<RecipeIngredient, RecipeIngredientResponse>()
            .ForMember(
                d => d.Name,
                opt => opt.MapFrom(s => s.Ingredient.Name));

        // Request → Entity
        CreateMap<CreateRecipeRequest, Recipe>()
            .ForMember(d => d.RecipeId, opt => opt.Ignore())
            .ForMember(d => d.AuthorId, opt => opt.Ignore())
            .ForMember(d => d.Author, opt => opt.Ignore())
            .ForMember(d => d.Cuisine, opt => opt.Ignore())
            .ForMember(d => d.Category, opt => opt.Ignore())
            .ForMember(d => d.CuisineId, opt => opt.Ignore())
            .ForMember(d => d.CategoryId, opt => opt.Ignore())
            .ForMember(d => d.RecipeIngredients, opt => opt.Ignore())
            .ForMember(d => d.UserFavorites, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore());

        CreateMap<UpdateRecipeRequest, Recipe>()
            .ForMember(d => d.Title, o => o.Condition(s => s.Title != null))
            .ForMember(d => d.PrepTimeMinutes, o => o.Condition(s => s.PrepTimeMinutes != null))
            .ForMember(d => d.CookTimeMinutes, o => o.Condition(s => s.CookTimeMinutes != null))
            .ForMember(d => d.Servings, o => o.Condition(s => s.Servings != null))
            .ForMember(d => d.Instructions, o => o.Condition(s => s.Instructions != null))
            .ForMember(d => d.ImageUrl, o => o.Condition(s => s.ImageUrl != null))
            .ForMember(d => d.CuisineId, o => o.Ignore())
            .ForMember(d => d.CategoryId, o => o.Ignore());
        // CuisineId/CategoryId and ingredient ids are resolved via id-or-name
        // in RecipeService, so nullable ints are never mapped onto the entity.
        // Author, navigations and timestamps stay unchanged.
    }
}