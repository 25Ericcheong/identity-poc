namespace IdentityServer;

public static class Constants
{
    public static class Urls
    {
        public const string ThisIdentityServer = "https://localhost:5001";

        public const string Mvc = "https://localhost:44330";
        
        public const string BffSignInOidc = "https://localhost:7095/signin-oidc";
        
        public const string BffSignOutOidc = "https://localhost:7095/signout-callback-oidc";
    }

    public static class AllowedScopes
    {
        public const string FrameworkApiScope = "FrameworkApiScope";
        
        public const string CoreApiScope = "CoreApiScope";
    }
}