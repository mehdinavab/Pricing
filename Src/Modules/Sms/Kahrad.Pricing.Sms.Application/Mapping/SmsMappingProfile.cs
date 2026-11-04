using AutoMapper;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Domain.Entities;

namespace Kahrad.Pricing.Sms.Application.Mapping;

public sealed class SmsMappingProfile : Profile
{
    public SmsMappingProfile()
    {
        CreateMap<SmsMessage, SmsMessageDto>();
    }
}
