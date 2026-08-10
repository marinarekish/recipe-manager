using AutoMapper;
using RecipeManager.Application.Contracts.Favorites;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Mapping;

public class FavoriteMappingProfile : Profile
{
    public FavoriteMappingProfile()
    {
        CreateMap<UserFavorite, FavoriteRecipeResponse>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Recipe.Title))
            .ForMember(d => d.AddedAt, opt => opt.MapFrom(s => s.CreatedAt));
    }
}