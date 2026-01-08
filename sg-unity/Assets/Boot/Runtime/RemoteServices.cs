using YooAsset;

public class RemoteServices : IRemoteServices
{
    private string Host { get; }
    private string FallbackHost { get; }

    public RemoteServices(string host, string fallbackHost)
    {
        Host = host;
        FallbackHost = fallbackHost;
    }
    
    public string GetRemoteMainURL(string fileName)
    {
        return $"{Host}/{fileName}";
    }

    public string GetRemoteFallbackURL(string fileName)
    {
        return $"{FallbackHost}/{fileName}";
    }
}