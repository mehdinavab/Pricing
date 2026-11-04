using AutoMapper;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;
using Kahrad.Pricing.Identity.Domain.Entities;

namespace Kahrad.Pricing.Identity.Application.Mapping
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<ApplicationRole, RoleDto>();
        }
    }
}
