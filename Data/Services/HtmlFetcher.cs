using System.Net.Http;
using System.Threading.Tasks;
namespace ndisforms.Data.Services
{
   

    public class HtmlFetcher
    {
        public async Task<string> FetchHtmlAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var htmlContent = await client.GetStringAsync(url);
                return htmlContent;
            }
        }
    }

}
