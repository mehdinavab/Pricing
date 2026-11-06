using MAHAK.Integration.Api.Security;
using Microsoft.Data.SqlClient;

namespace MAHAK.Integration.Api.Data;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    public SqlConnection Create(BrandContext brand)
    {
        var integratedSecurity = brand.AuthenticationMode.Equals(
            "IntegratedSecurity",
            StringComparison.OrdinalIgnoreCase);
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = brand.DataSource,
            InitialCatalog = brand.Database,
            IntegratedSecurity = integratedSecurity,
            Encrypt = brand.Encrypt
                ? SqlConnectionEncryptOption.Mandatory
                : SqlConnectionEncryptOption.Optional,
            TrustServerCertificate = brand.TrustServerCertificate,
            ConnectTimeout = brand.ConnectTimeoutSeconds,
            MaxPoolSize = brand.MaxPoolSize,
            ApplicationName = "MAHAK.Integration.Api",
            PersistSecurityInfo = false
        };
        if (!integratedSecurity)
        {
            builder.UserID = brand.UserId;
            builder.Password = brand.Password;
        }

        return new SqlConnection(builder.ConnectionString);
    }
}
