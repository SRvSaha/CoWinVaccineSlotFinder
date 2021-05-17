using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CoWin.Core.Models
{
    public class VersionModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("published_at")]
        public DateTimeOffset PublishedAt { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
       
    }

    public partial class Asset
    {
        [JsonProperty("download_count")]
        public long DownloadCount { get; set; }

        [JsonProperty("browser_download_url")]
        public Uri BrowserDownloadUrl { get; set; }
    }
}
