namespace CoreApi;

public static class Constants
{
    public static class Urls
    {
        public const string IdentityServer = "https://localhost:5001";

        public const string ThisCoreApi = "https://localhost:44330";
    }
    
    public static class AllowedScopes
    {
        public const string CoreApiScope = "CoreApiScope";
    }
    
    public static class ClaimTypes
    {
        public const string Scope = "scope";
    }

    public static class PolicyName
    {
        public const string ApiScope = "ApiScope";
    }
        
}