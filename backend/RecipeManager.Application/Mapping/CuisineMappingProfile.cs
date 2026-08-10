using AutoMapper;
using RecipeManager.Application.Contracts.Cuisines;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Mapping;

public class CuisineMappingProfile : Profile
{
    public  CuisineMappingProfile()
    {
        CreateMap<Cuisine, CuisineResponse>();
    }
}