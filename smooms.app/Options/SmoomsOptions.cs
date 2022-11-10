namespace smooms.app.Options;

public class SmoomsOptions
{
    public string InitialRealmName { get; set; } = "master";
    public string InitialRealmDisplayName { get; set; } = "Master Realm";
    public string InitialRealmDescription { get; set; } = "Automatically created initial master realm";
    public string InitialAdminName { get; set; } = "admin";
    public string? InitialAdminPassword { get; set; }
    public string? InitialAdminEmail { get; set; }

    public string? ApplicationPepper { get; set; }
}