using AutoMapper;
using RecipeManager.Application.Contracts.Roles;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Mapping;

public class RoleMappingProfile : Profile
{
    public  RoleMappingProfile()
    {
        CreateMap<UserRole, RoleResponse>()
            .ForMember(d => d.RoleId, opt => opt.MapFrom(s => s.Role.RoleId))
            .ForMember(d =>  d.Name, opt => opt.MapFrom(s => s.Role.Name));
    }
}