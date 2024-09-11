using System.Collections.Generic;

namespace Polecola.Youtube.Api.Models
{
    public class VideoSearchResult
    {
        public List<Video> Videos { get; set; } = new List<Video>();
        public SearchInfo SearchInfo { get; set; } = new SearchInfo();
    }
}