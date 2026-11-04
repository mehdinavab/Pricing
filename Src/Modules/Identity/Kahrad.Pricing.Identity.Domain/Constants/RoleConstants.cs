namespace Kahrad.Pricing.Identity.Domain.Constants
{
    public static class RoleConstants
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Contributor = "Contributor";

        public static readonly IReadOnlyCollection<string> All =
        [
                SuperAdmin,
                Admin,
                Contributor
        ];
    }
}
