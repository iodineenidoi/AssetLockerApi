namespace AssetLockerApi.Core;

public class Project
{
    private Dictionary<string, AssetData> _assets;

    public Project()
    {
        _assets = new Dictionary<string, AssetData>();
    }

    public IsLockedResponse IsLocked(IsLockedRequest isLockedRequest)
    {
        string path = isLockedRequest.Asset;
        string user = isLockedRequest.User;
        
        IsLockedResponse response = new IsLockedResponse();
        if (_assets.ContainsKey(path) && _assets[path].User != user)
        {
            response.Result = true;
            response.Data = _assets[path];
            return response;
        }

        AssetData? lockedFolder = _assets.Values
            .Where(asset => asset.IsFolder() && asset.User != user)
            .FirstOrDefault(asset => path.StartsWith(asset.Path));
        
        if (lockedFolder != null)
        {
            response.Result = true;
            response.Data = lockedFolder;
            return response;
        }

        response.Result = false;
        response.Data = new AssetData("", "", "", "", DateTime.MinValue);
        return response;
    }

    public LockAssetsResponse LockAssets(LockAssetsRequest lockAssetsRequest)
    {
        List<AssetData> savedAssets = new List<AssetData>();
        List<AssetData> failedAssets = new List<AssetData>();

        string user = lockAssetsRequest.User;
        string message = lockAssetsRequest.Message;
        string gitBrunch = lockAssetsRequest.Brunch;
        DateTime lockTime = lockAssetsRequest.LockTime;
        
        foreach (string path in lockAssetsRequest.Assets)
        {
            if (_assets.ContainsKey(path))
            {
                failedAssets.Add(_assets[path]);
                continue;
            }

            if (_assets.Values.Any(asset => asset.IsFolder() && path.StartsWith(asset.Path)))
            {
                failedAssets.Add(_assets[path]);
                continue;
            }

            _assets.Add(path, new AssetData(path, user, message, gitBrunch, lockTime));
            savedAssets.Add(_assets[path]);
        }

        LockAssetsResponse response = new LockAssetsResponse
        {
            Saved = savedAssets.Select(asset => asset.Path).ToList(),
            Failed = failedAssets.ToList()
        };
        return response;
    }

    public UnlockAssetsResponse UnlockAssets(UnlockAssetsRequest unlockAssetsRequest)
    {
        string user = unlockAssetsRequest.User;
        
        List<AssetData> otherUsersFolders = _assets.Values
            .Where(asset => asset.IsFolder() && asset.User != user).ToList();

        List<string> unlocked = new List<string>();
        List<string> wasntLockedPaths = new List<string>();
        List<AssetData> lockedByOtherUser = new List<AssetData>();
        
        foreach (string path in unlockAssetsRequest.Assets)
        {
            if (_assets.ContainsKey(path))
            {
                if (_assets[path].User != unlockAssetsRequest.User)
                {
                    lockedByOtherUser.Add(_assets[path]);
                    continue;
                }
            }
            else
            {
                AssetData? lockedFolder = otherUsersFolders.FirstOrDefault(asset => path.StartsWith(asset.Path));
                if (lockedFolder != null)
                {
                    lockedByOtherUser.Add(lockedFolder);
                    continue;
                }
                
                wasntLockedPaths.Add(path);
            }

            unlocked.Add(path);
            _assets.Remove(path);
        }
        
        UnlockAssetsResponse response = new UnlockAssetsResponse
        {
            Unlocked = unlocked.ToList(),
            WasntLocked = wasntLockedPaths.ToList(),
            LockedByOtherUser = lockedByOtherUser.ToList()
        };
        return response;
    }

    public GetAllResponse GetAll()
    {
        GetAllResponse response = new GetAllResponse
        {
            LockedAssets = _assets.Values.ToList()
        };
        return response;
    }
}