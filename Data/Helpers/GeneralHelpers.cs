namespace ndisforms.Data.Helpers
{
    public static class GeneralHelpers
    {
        public static string GenerateIrLink(Guid AutoSaveDraftGuid, string baseUrl)
        {
            //var baseUrl = "https://localhost:7133/";
            //var baseUrl = HttpContext.Request.BaseUrl();
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl = string.Concat(baseUrl, "/");
            }
            var missionLink = $"{baseUrl}viewir?id={AutoSaveDraftGuid}";
            return missionLink;
        }
    }
}
