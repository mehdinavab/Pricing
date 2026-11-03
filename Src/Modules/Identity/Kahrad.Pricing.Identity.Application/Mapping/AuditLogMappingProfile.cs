using AutoMapper;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;
using Kahrad.Pricing.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Application.Mapping
{
    public class AuditLogMappingProfile : Profile
    {
        public AuditLogMappingProfile()
        {
            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(x => x.UserId, options => options.MapFrom(source => source.CreatedById))
                .ForMember(x => x.EntityId, options => options.MapFrom(
                    source => source.EntityPublicId.HasValue
                        ? source.EntityPublicId.Value.ToString()
                        : null))
                .ForMember(x => x.Description, options => options.MapFrom(source => source.Description));
        }
    }
}
