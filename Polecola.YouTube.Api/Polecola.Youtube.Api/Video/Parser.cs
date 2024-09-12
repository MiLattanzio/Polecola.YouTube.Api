using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Polecola.Youtube.Api.Video
{
    public class Parser
    {
        private const string SearchResultsFeature = "ytInitialData";

        private static Models.Video? VideoRenderer(JObject renderer)
        {
            var id = renderer["videoId"]?.Value<string>();
            if (string.IsNullOrEmpty(id)) return null;
            var uri = "https://www.youtube.com/watch?v=" + id;
            var title = renderer["title"]?[(object) "runs"]?[(object) 0]?[(object) "text"]?.Value<string>();
            if (string.IsNullOrEmpty(title)) return null;
            var thumbnailUrl = renderer["thumbnail"]?[(object) "thumbnails"]?[(object) 0]?[(object) "url"]?.Value<string>();
            if (string.IsNullOrEmpty(thumbnailUrl)) return null;
            var duration = "";
            if (renderer.ContainsKey("lengthText"))
                duration = renderer["lengthText"]?[(object) "simpleText"]?.Value<string>()?.Replace(".", ":");
            var author = renderer["longBylineText"]?[(object) "runs"]?[(object) 0]?[(object) "text"]?.Value<string>();
            if (string.IsNullOrEmpty(author)) return null;
            if (string.IsNullOrEmpty(duration)) return null;
            var youtubeVideo = new Models.Video()
            {
                Id = id,
                ChannelTitle = author,
                Duration = Utils.TimeSpan.Parse(duration),
                ThumbnailUrl = thumbnailUrl,
                Title = title,
                Url = uri
            };
            return youtubeVideo;
        }
        
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
                    var youtubeVideo = VideoRenderer(renderer);
                    if (youtubeVideo == null) continue;
                    data.Add(youtubeVideo);
                }
            }
            return data;
        }

        public static List<Models.Video> ParseNextJson(string pageContent)
        {
            var page = JObject.Parse(pageContent);
            var item1 = page["onResponseReceivedCommands"]?[0]?["appendContinuationItemsAction"];
            if (null == item1) return new List<Models.Video>();
            if (item1["continuationItems"] is not JArray continuationItems) return new List<Models.Video>();
            var data = new List<Models.Video>();
            foreach (var continuation in continuationItems)
            {
                if (continuation["itemSectionRenderer"]?["contents"] is JArray contents)
                {
                    foreach (var jToken in contents)
                    {
                        var jObj = (JObject)jToken;
                        if (jObj.ContainsKey("videoRenderer"))
                        {
                            var renderer = jObj["videoRenderer"]?.Value<JObject>();
                            if (null == renderer) continue;
                            var youtubeVideo = VideoRenderer(renderer);
                            if (youtubeVideo == null) continue;
                            data.Add(youtubeVideo);
                        }
                    }
                }
            }
            return data;
        }
    }
}