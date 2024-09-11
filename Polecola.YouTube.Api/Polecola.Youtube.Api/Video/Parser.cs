using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Polecola.Youtube.Api.Video
{
    public class Parser
    {
        private const string SearchResultsFeature = "ytInitialData";

        public static List<Models.Video> ParseHtml(string pageContent)
        {
            var startIndex = pageContent.IndexOf(SearchResultsFeature, StringComparison.Ordinal) + SearchResultsFeature.Length + 3;
            var str = pageContent.Substring(startIndex, pageContent.Length - startIndex);
            var length = str.IndexOf("};", StringComparison.Ordinal) + 1;
            var jarray = JObject.Parse(str.Substring(0, length))["contents"]?[(object) "twoColumnSearchResultsRenderer"]?[(object) "primaryContents"]?[(object) "sectionListRenderer"]?[(object) "contents"]?[(object) 0]?[(object) "itemSectionRenderer"]?[(object) "contents"]?.Value<JArray>();
            var data = new List<Models.Video>();
            if (null == jarray) return data;
            foreach (var jToken in jarray)
            {
                var jObj = (JObject)jToken;
                if (jObj.ContainsKey("videoRenderer"))
                {
                    var renderer = jObj["videoRenderer"]?.Value<JObject>();
                    if (null == renderer) continue;
                    var id = renderer["videoId"]?.Value<string>();
                    if (string.IsNullOrEmpty(id)) continue;
                    var uri = "https://www.youtube.com/watch?v=" + id;
                    var title = renderer["title"]?[(object) "runs"]?[(object) 0]?[(object) "text"]?.Value<string>();
                    if (string.IsNullOrEmpty(title)) continue;
                    var thumbnailUrl = renderer["thumbnail"]?[(object) "thumbnails"]?[(object) 0]?[(object) "url"]?.Value<string>();
                    if (string.IsNullOrEmpty(thumbnailUrl)) continue;
                    var duration = "";
                    if (renderer.ContainsKey("lengthText"))
                        duration = renderer["lengthText"]?[(object) "simpleText"]?.Value<string>()?.Replace(".", ":");
                    var author = renderer["longBylineText"]?[(object) "runs"]?[(object) 0]?[(object) "text"]?.Value<string>();
                    if (string.IsNullOrEmpty(author)) continue;
                    if (string.IsNullOrEmpty(duration)) continue;
                    var youtubeVideo = new Models.Video()
                    {
                        Id = id,
                        ChannelTitle = author,
                        Duration = Utils.TimeSpan.Parse(duration),
                        ThumbnailUrl = thumbnailUrl,
                        Title = title,
                        Url = uri
                    };
                    data.Add(youtubeVideo);
                }
            }
            return data;
        }
    }
}