namespace MvcApp.RequestResponse
{
    public class FrontendResponse
    {
        public bool IsAuthenticated { get; set; }
        
        public FrontEndAssets Assets { get; set; }
    }
    
    public class FrontEndAssets
    {
        public string HeadContent { get; set; }
        public string BodyContent { get; set; }
    }
}