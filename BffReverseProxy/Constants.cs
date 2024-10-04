namespace BffReverseProxy;

public static class Constants
{
    public static class Urls
    {
        public const string IdentityServer = "https://localhost:5001";

        public const string Mvc = "https://localhost:44330";
        
        public const string VueSpaFrontend = "https://localhost:5173";
    }

    public static class AllowedScopes
    {
        public const string FrameworkApiScope = "FrameworkApiScope";
        
        public const string CoreApiScope = "CoreApiScope";
    }
}