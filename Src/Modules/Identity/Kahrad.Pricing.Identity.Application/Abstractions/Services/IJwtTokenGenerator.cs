using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

public interface IJwtTokenGenerator
{
    Task<TokenResponseDto> GenerateAsync(ApplicationUser user);
}
