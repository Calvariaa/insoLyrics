using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace insoLyrics.Lyrics.getLyrics
{
    public class SearchLyrics
    {
        string Id = string.Empty;
        public SearchLyrics(string id) { Id = id; }
        public SearchLyrics(int id) { Id = id.ToString(); }

        public string GetLyrics()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            var getLyrics = httpClient.GetStringAsync($@"https://music.163.com/api/song/lyric?id={Id}&lv=1&kv=1&tv=-1")
                .Result.ToString();

            var jsonLyrics = JsonNode.Parse(getLyrics);
            var res = jsonLyrics!["lrc"]!["lyric"]!.ToString();
            return res;
        }
    }
}
