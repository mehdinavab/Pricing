using MAHAK.Integration.Api.Security;
using Microsoft.Data.SqlClient;

namespace MAHAK.Integration.Api.Data;

public interface ISqlConnectionFactory
{
    SqlConnection Create(BrandContext brand);
}
