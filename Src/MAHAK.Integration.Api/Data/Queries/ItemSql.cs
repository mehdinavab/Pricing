namespace MAHAK.Integration.Api.Data.Queries;

public static class ItemSql
{
    public const string ReturnedItemsOnly = """
        SELECT DISTINCT TOP (@Take)
            CONVERT(nvarchar(128), g.code) AS Code
        FROM dbo.GoodInf AS g
        WHERE g.code IS NOT NULL
          AND EXISTS (
              SELECT 1
              FROM dbo.BackFact_Detail AS d
              WHERE d.kala_code = g.code
          )
        ORDER BY Code;
        """;
}
