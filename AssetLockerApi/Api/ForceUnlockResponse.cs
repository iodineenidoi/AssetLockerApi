namespace AssetLockerApi.Api;

public class ForceUnlockResponse
{
    public string Asset { get; set; }
    public int RequestsLeft { get; set; }
}