namespace IdentityServer;

public class Constants
{
    public static class Urls
    {
        public const string ThisIdentityServer = "https://localhost:5001";

        public const string Mvc = "https://localhost:44330";
    }

    public static class AllowedScopes
    {
        public const string FrameworkApiScope = "FrameworkApiScope";
        
        public const string CoreApiScope = "CoreApiScope";
    }
}