namespace FrameworkApi

{ 
    public static class Constants {
        public static class Urls
        {
            public const string IdentityServer = "https://localhost:5001";

            public const string ThisMvc = "https://localhost:44330";

            public const string ApiFramework = "https://localhost:44329/api";
        }
    
        public static class AllowedScopes
        {
            public const string FrameworkApiScope = "FrameworkApiScope";
        }
    
        public static class ClaimTypes
        {
            public const string Scope = "scope";
        }
    }
}
