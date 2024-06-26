namespace Necnat.Abp.NnMgmtAuthorization;

public static class NnMgmtAuthorizationDbProperties
{
    public static string DbTablePrefix { get; set; } = "NnMgmtAuthorization";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "NnMgmtAuthorization";
}
