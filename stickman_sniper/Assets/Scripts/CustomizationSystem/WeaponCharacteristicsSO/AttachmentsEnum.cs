public enum AttachmentsTab
{
    None = 0,
    Scope = 1,
    Muzzle = 2,
    Laser = 3,
    Grip = 4,
    Magazine = 5
}

public enum InventoryTab
{
    None = 0,
    SniperRifle = 1
}

public static class AttachmentsTabExtensions
{
    public static AttachmentsTab ToAttachmentTab(this string key) => key switch
    {
        "scope" => AttachmentsTab.Scope,
        "muzzle" => AttachmentsTab.Muzzle,
        "laser" => AttachmentsTab.Laser,
        "grip" => AttachmentsTab.Grip,
        "magazine" => AttachmentsTab.Magazine
    };
}