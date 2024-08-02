namespace ndisforms.Data.Helpers
{
    public static class HttpRequestExtensions
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string? BaseUrl(this HttpRequest req)
        {
            if (req == null) return null;
            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }

        public static string? GetBaseUrl
        {
            get
            {
                HttpRequest req = _httpContextAccessor.HttpContext.Request;
                if (req == null) return null;
                var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
                if (uriBuilder.Uri.IsDefaultPort)
                {
                    uriBuilder.Port = -1;
                }

                var rtnstr = uriBuilder.Uri.AbsoluteUri;
                if (!rtnstr.EndsWith("/")) { rtnstr = string.Concat(rtnstr, "/"); }

                return rtnstr;
            }

        }

    }
}
