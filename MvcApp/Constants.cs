namespace MvcApp
{
    public static class Urls
    {
        public const string IdentityServer = "https://localhost:5001";

        public const string ThisMvc = "https://localhost:44330";

        public const string ApiFramework = "https://localhost:44329/api";

        public const string ApiCore = "https://localhost:7055/api";
    }
    
    public static class AllowedScopes
    {
        public const string FrameworkApiScope = "FrameworkApiScope";
        
        public const string CoreApiScope = "CoreApiScope";
    }
}