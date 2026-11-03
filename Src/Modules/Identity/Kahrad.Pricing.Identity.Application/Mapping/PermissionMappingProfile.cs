using AutoMapper;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using Kahrad.Pricing.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Application.Mapping
{
    public class PermissionMappingProfile : Profile
    {
        public PermissionMappingProfile()
        {
            CreateMap<Permission, PermissionDto>();
        }
    }
}
