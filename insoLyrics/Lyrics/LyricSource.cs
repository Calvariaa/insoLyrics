using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using insoLyrics.Beatmap;
using insoLyrics.Lyrics.getLyrics;
using insoLyrics.Lyrics.getMetadata;

namespace insoLyrics.Lyrics
{
    public abstract class LyricSource
    {
        public static readonly List<LyricSource> sources = new List<LyricSource>();

        // public static IEnumerable<Lyric> GetLyricsAsync(BeatmapMetadata bm) => sources.Select(i => i.GetLyric(bm));

        public static Lyric GetLyric(BeatmapMetadata bm)
        {

            var meta = new SearchMetadata(bm);
            var id = meta.GetMetadata();

            var lrc = new SearchLyrics(id);
            var res = lrc.GetLyrics();

            Lyric lyric = new Lyric(res.Split("\n", StringSplitOptions.RemoveEmptyEntries));
            return lyric;
        }
    }
}
