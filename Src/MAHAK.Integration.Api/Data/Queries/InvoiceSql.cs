namespace MAHAK.Integration.Api.Data.Queries;

public static class InvoiceSql
{
    public const string CurrentNonReturnedProjection = """
        SELECT DISTINCT TOP (@Take)
            CONVERT(nvarchar(256), PRF.Fax) AS Fax,
            CONVERT(nvarchar(128), FM.Shakhs_Code) AS PersonCode,
            CONVERT(nvarchar(128), f.kala_code) AS ProductCode,
            CONVERT(nvarchar(128), f.code) AS InvoiceCode
        FROM dbo.Fact_Fo_Detail AS f
        LEFT JOIN dbo.BackFact_Detail AS bf
            ON bf.sanad_code = f.code
           AND bf.kala_code = f.kala_code
        LEFT JOIN dbo.Fact_Fo AS FM
            ON FM.Code = f.code
        LEFT JOIN dbo.PerInf AS PRF
            ON PRF.code = FM.Shakhs_Code
        WHERE bf.sanad_code IS NULL
          AND PRF.GrpCode IN (4, 5, 6)
          AND f.kala_code > @ProductCodeThreshold
        ORDER BY InvoiceCode, ProductCode, PersonCode, Fax;
        """;
}
