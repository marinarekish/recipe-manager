using AutoMapper;
using RecipeManager.Application.Contracts.Favorites;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Mapping;

public class FavoriteMappingProfile : Profile
{
    public FavoriteMappingProfile()
    {
        CreateMap<UserFavorite, FavoriteRecipeResponse>()
            .ForMember(d => d.RecipeId, o => o.MapFrom(s => s.RecipeId))
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Recipe.Title))
            .ForMember(d => d.AddedAt, o => o.MapFrom(s => s.CreatedAt));
    }
}