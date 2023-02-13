namespace AssetLockerApi.Core;

public class ForceUnlockHelper
{
    private class UnlockRequest
    {
        public string User { get; }
        public DateTime RequestTime { get; }
        
        public UnlockRequest(string user)
        {
            User = user;
            RequestTime = DateTime.Now;
        }
    }

    private const int MaxSecondsToCountRequests = 180;
    private const int RequestsRequiredToUnlock = 3;

    private Dictionary<string, List<UnlockRequest>> _unlockRequests;

    public ForceUnlockHelper()
    {
        _unlockRequests = new Dictionary<string, List<UnlockRequest>>();
    }

    public int ForceUnlockByUser(string path, string user)
    {
        if (!_unlockRequests.ContainsKey(path))
            _unlockRequests[path] = new List<UnlockRequest>();

        UnlockRequest unlockRequest = new UnlockRequest(user);
        
        int currentUserIndex = _unlockRequests[path].FindIndex(x => x.User == user);
        if (currentUserIndex == -1)
        {
            _unlockRequests[path].Add(unlockRequest);
        }
        else
        {
            _unlockRequests[path][currentUserIndex] = unlockRequest;
        }

        _unlockRequests[path].RemoveAll(IsTimeToRemoveRequest);

        int result = RequestsRequiredToUnlock - _unlockRequests[path].Count;
        return Math.Max(result, 0);
    }

    private bool IsTimeToRemoveRequest(UnlockRequest request)
    {
        return (DateTime.Now - request.RequestTime).Seconds > MaxSecondsToCountRequests;
    }
}