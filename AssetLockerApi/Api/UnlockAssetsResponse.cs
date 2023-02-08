namespace AssetLockerApi.Api;

public class UnlockAssetsResponse
{
    public List<string> Unlocked { get; set; }
    public List<string> WasntLocked { get; set; }
    public List<AssetData> LockedByOtherUser { get; set; }
}