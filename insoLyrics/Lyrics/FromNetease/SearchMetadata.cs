using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using insoLyrics.Beatmap;
using System.Text.Json.Nodes;

namespace insoLyrics.Lyrics.getMetadata
{
    public class SearchMetadata
    {
        string Title;
        string Artist;
        public SearchMetadata()
        {
            Title = @"Unknown";
            Artist = @"Unknown";
        }
        public SearchMetadata(BeatmapMetadata map)
        {
            Title = map.TitleUnicode ?? map.Title;
            Artist = map.ArtistUnicode ?? map.Artist;
        }
        public SearchMetadata(string t, string a)
        {
            Title = t;
            Artist = a;
        }
        public string GetMetadata()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            httpClient.Timeout = new TimeSpan(0, 0, 5);
            var getMeta = httpClient.GetStringAsync($@"https://music.163.com/api/search/get/web?csrf_token=hlpretag&hlposttag&s={Title}%20{Artist}&type=1&offset=0&total=true&limit=20")
                .Result.ToString();

            var jsonMeta = JsonNode.Parse(getMeta);
            Console.WriteLine(jsonMeta!["code"]!);
            Console.WriteLine(jsonMeta!["result"]!["songCount"]!);

            var res = string.Empty;
            if (((uint)jsonMeta!["code"]!) == 200 && ((uint)jsonMeta!["result"]!["songCount"]!) != 0)
            {
                res = jsonMeta!["result"]!["songs"]![0]!.ToJsonString().Split(',')[0].Split(':')[1];

            }

            return res;
        }
    }
}
