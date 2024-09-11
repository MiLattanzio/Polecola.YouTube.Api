using System;

namespace Polecola.Youtube.Api.Models
{
    public class Video
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string ChannelTitle { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public string Url { get; set; } = string.Empty;
    }
}