namespace BookmarkrV13.ServiceAgents.BookmarkrSyncrServiceAgent;

public class AccessTokenNotFoundException : Exception
{
    public string AccessToken { get; }

    public AccessTokenNotFoundException(string accessToken) : base($"The specified access token '{accessToken}' was not found.")
    {
        AccessToken = accessToken;
    }
}

public class AccessTokenInvalidException : Exception
{
    public string AccessToken { get; }

    public AccessTokenInvalidException(string accessToken) : base($"The specified access token '{accessToken}' is invalid.")
    {
        AccessToken = accessToken;
    }
}

public class AccessTokenExpiredException : Exception
{
    public string AccessToken { get; }

    public AccessTokenExpiredException(string accessToken) : base($"The specified access token '{accessToken}' is expired.")
    {
        AccessToken = accessToken;
    }
}
