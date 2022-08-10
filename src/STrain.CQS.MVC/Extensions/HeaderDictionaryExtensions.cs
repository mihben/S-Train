namespace Microsoft.AspNetCore.Http
{
    public static class HeaderDictionaryExtensions
    {
        public static string GetRequestType(this IHeaderDictionary headers)
        {
            return headers[STrain.CQS.MVC.Constants.Headers.RequestType];
        }
    }
}
