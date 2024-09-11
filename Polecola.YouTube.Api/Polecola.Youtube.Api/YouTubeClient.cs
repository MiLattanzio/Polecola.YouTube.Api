using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Polecola.Youtube.Api.Models;

namespace Polecola.Youtube.Api
{
    public sealed class YouTubeClient
    {
        private const string SearchBaseUrl = "https://www.youtube.com/results?search_query=";
        private readonly HttpClient _httpClient;

        public YouTubeClient() : this(new HttpClient())
        {
        }

        public YouTubeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<VideoSearchResult?> SearchVideoAsync(string query, int page = 0, int retry = 3)
        {
            var encodedKeywords = HttpUtility.UrlEncode(query);
            var searchUrl = $"{SearchBaseUrl}{encodedKeywords}";
            do
            {
                var response = await _httpClient.GetAsync(searchUrl);
                if (!response.IsSuccessStatusCode)
                {
                    retry--;
                    continue;
                }

                var html = await response.Content.ReadAsStringAsync();
                if (html == null) continue;
                var info = SearchInfo.Parser.ParseHtml(html);
                if (info == null) continue;
                var videos = Video.Parser.ParseHtml(html);
                if (videos is not { Count: > 0 }) continue;
                return new VideoSearchResult()
                {
                    SearchInfo = info,
                    Videos = videos
                };
            } while (retry >= 0);

            return null;
        }
    }
}