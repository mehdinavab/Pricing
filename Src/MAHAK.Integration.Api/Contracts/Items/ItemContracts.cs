using MAHAK.Integration.Api.Contracts.Common;

namespace MAHAK.Integration.Api.Contracts.Items;

public enum ItemSyncScope
{
    AllItems = 1,
    ReturnedItemsOnly = 2
}

public sealed record ItemSyncRequest : SyncRequest
{
    public ItemSyncScope? Scope { get; init; }
}

public sealed record AccountingItem(string Code);
