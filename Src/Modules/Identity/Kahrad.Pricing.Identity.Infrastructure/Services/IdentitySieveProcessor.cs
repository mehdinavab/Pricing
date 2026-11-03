using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Identity.Infrastructure.Services;

public sealed class IdentitySieveProcessor(IOptions<SieveOptions> options)
    : SieveProcessor(options)
{
    protected override SievePropertyMapper MapProperties(
        SievePropertyMapper mapper)
    {
        mapper = base.MapProperties(mapper);

        mapper.Property<ApplicationRole>(role => role.Name)
            .CanFilter()
            .CanSort();
        mapper.Property<ApplicationRole>(role => role.Id)
            .CanSort();
        mapper.Property<ApplicationUser>(user => user.Id)
            .CanSort();
        mapper.Property<AuditLog>(auditLog => auditLog.Id)
            .CanSort();

        return mapper;
    }
}
