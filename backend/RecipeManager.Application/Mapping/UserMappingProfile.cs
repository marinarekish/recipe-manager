using AutoMapper;
using RecipeManager.Application.Contracts.Roles;
using RecipeManager.Application.Contracts.Users;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(
                d => d.Roles,
                opt => opt.MapFrom(s => s.UserRoles));

        CreateMap<UserRole, RoleResponse>()
            .ForMember(
                d => d.RoleId,
                opt => opt.MapFrom(s => s.RoleId))
            .ForMember(
                d => d.Name,
                opt => opt.MapFrom(s => s.Role.Name));
    }
}