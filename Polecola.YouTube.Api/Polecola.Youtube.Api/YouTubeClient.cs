using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polecola.Youtube.Api.Models;

namespace Polecola.Youtube.Api
{
    /// <summary>
    /// Provides methods to interact with the YouTube search API.
    /// </summary>
    public sealed class YouTubeClient
    {
        private const string SearchBaseUrl = "https://www.youtube.com/results?search_query=";
        private const string SearchNextUrl = "https://www.youtube.com/youtubei/v1/search?key=";
        
        private readonly HttpClient _httpClient;

        public YouTubeClient() : this(new HttpClient())
        {
        }

        public YouTubeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Searches for videos on YouTube based on the provided query string.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="page">The page number to retrieve. Defaults to 0.</param>
        /// <param name="retry">The number of retries if the search fails. Defaults to 3.</param>
        /// <returns>A <see cref="VideoSearchResult"/> object containing the search results, or <c>null</c> if the search fails after the specified number of retries.</returns>
        public async Task<VideoSearchResult?> SearchVideoAsync(string query, int page = 0, int retry = 3)
        {
            var encodedKeywords = HttpUtility.UrlEncode(query);
            var searchUrl = $"{SearchBaseUrl}{encodedKeywords}&sp=EgIQAQ%3D%3D";
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
                if (videos is not { Count: > 0 })
                {
                    retry--;
                    continue;
                }
                while (page > 0)
                {
                    var result = await NextVideosAsync(info, retry);
                    videos = result?.Videos ?? new List<Models.Video>();
                    info = result?.SearchInfo ?? info;
                    page--;
                }
                return new VideoSearchResult()
                {
                    SearchInfo = info,
                    Videos = videos
                };
            } while (retry >= 0);

            return null;
        }

        /// <summary>
        /// Retrieves the next page of YouTube videos based on the provided search information.
        /// </summary>
        /// <param name="searchInfo">The current search information containing context and continuation tokens.</param>
        /// <param name="retry">The number of retries if the request for the next set of videos fails. Defaults to 3.</param>
        /// <returns>A <see cref="VideoSearchResult"/> object containing the next set of search results, or <c>null</c> if the request fails after the specified number of retries.</returns>
        public async Task<VideoSearchResult?> NextVideosAsync(Models.SearchInfo searchInfo, int retry = 3)
        {
            var searchUrl = $"{SearchNextUrl}{searchInfo.ApiToken}";
            do
            {
                var param = new
                {
                    context = searchInfo.Context,
                    continuation = searchInfo.ContinuationToken
                };
                var json = JsonConvert.SerializeObject(param, Formatting.Indented);
                var response = await _httpClient.PostAsync(searchUrl, new StringContent(json, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    retry--;
                    continue;
                }
                var jsonPage = await response.Content.ReadAsStringAsync();
                var videos = Video.Parser.ParseNextJson(jsonPage);
                if (videos is not { Count: > 0 })
                {
                    retry--;
                    continue;
                }
                return new VideoSearchResult()
                {
                    SearchInfo = searchInfo,
                    Videos = videos
                };
            }while (retry >= 0);
            return null;
        }
    }
}