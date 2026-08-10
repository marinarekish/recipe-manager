using AutoMapper;
using RecipeManager.Application.Contracts.Ingredients;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Mapping;

public class IngredientMappingProfile : Profile
{
    public IngredientMappingProfile()
    {
        CreateMap<Ingredient, IngredientResponse>();
        
        CreateMap<CreateIngredientRequest, Ingredient>()
            .ForMember(d => d.IngredientId, opt => opt.Ignore())
            .ForMember(d => d.RecipeIngredients, opt => opt.Ignore());
    }
}