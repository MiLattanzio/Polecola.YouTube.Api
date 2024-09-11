using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Polecola.Youtube.Api.SearchInfo
{
    public class Parser
    {
        public static Models.SearchInfo? ParseHtml(string page)
        {
            try
            {
                var ytInitData = page.Split("var ytInitialData =");
                if (ytInitData.Length > 1)
                {
                    var data = ytInitData[1].Split("</script>")[0][..^1];
                    string? apiToken = null;
                    var innerTubeApiKeys = page.Split("innertubeApiKey");
                    if (innerTubeApiKeys is { Length: > 0 })
                    {
                        apiToken = innerTubeApiKeys[1].Trim().Split(",")[0].Split('"')[2];
                    }
                    if (string.IsNullOrEmpty(apiToken)) return null;

                    var context = page.Split("INNERTUBE_CONTEXT") is { Length: > 0} innerTubeContexts ? 
                        JObject.Parse(innerTubeContexts[1].Trim()[2..^2]) : 
                        null;
                    if (context == null) return null;
                    var initdata = JObject.Parse(data);
                    return new Models.SearchInfo()
                    {
                        ApiToken = apiToken,
                        Context = context,
                        InitData = initdata,
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}