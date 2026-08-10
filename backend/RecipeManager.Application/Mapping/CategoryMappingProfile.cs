using AutoMapper;
using RecipeManager.Application.Contracts.Categories;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Mapping;

public class CategoryMappingProfile : Profile
{
    public  CategoryMappingProfile()
    {
        CreateMap<Category, CategoryResponse>();
    }
}