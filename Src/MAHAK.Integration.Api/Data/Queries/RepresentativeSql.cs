namespace MAHAK.Integration.Api.Data.Queries;

public static class RepresentativeSql
{
    public const string BoundedProjection = """
        SELECT DISTINCT TOP (@Take)
            CONVERT(nvarchar(128), p.code) AS Code,
            CONVERT(nvarchar(256), p.Fax) AS Fax,
            CONVERT(nvarchar(32), p.GrpCode) AS GroupCode
        FROM dbo.PerInf AS p
        WHERE p.code IS NOT NULL
          AND LEN(p.Fax) > 1
          AND p.GrpCode IN (4, 5, 6)
        ORDER BY Code, Fax, GroupCode;
        """;
}
