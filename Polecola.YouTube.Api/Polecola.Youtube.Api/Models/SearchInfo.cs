using Newtonsoft.Json.Linq;

namespace Polecola.Youtube.Api.Models
{
    public class SearchInfo
    {
        public JObject InitData { get; set; } = new JObject();
        public string ApiToken { get; set; } = string.Empty;
        public JObject Context { get; set; } = new JObject();
        
        public string ContinuationToken { get; set; } = string.Empty;
    }
}