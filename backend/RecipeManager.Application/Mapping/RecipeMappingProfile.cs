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
            .ForMember(d => d.RecipeIngredients, opt => opt.Ignore())
            .ForMember(d => d.UserFavorites, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore());

        CreateMap<UpdateRecipeRequest, Recipe>()
            .ForMember(d => d.RecipeId, opt => opt.Ignore())
            .ForMember(d => d.AuthorId, opt => opt.Ignore())
            .ForMember(d => d.Author, opt => opt.Ignore())
            .ForMember(d => d.Cuisine, opt => opt.Ignore())
            .ForMember(d => d.Category, opt => opt.Ignore())
            .ForMember(d => d.RecipeIngredients, opt => opt.Ignore())
            .ForMember(d => d.UserFavorites, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
    }
}