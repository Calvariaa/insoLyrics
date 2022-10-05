using System;

namespace insoLyrics.Lyrics
{
    public class LyricLine
    {
        public double Time { get; private set; }
        public string Text { get; private set; }

        public LyricLine() { }

        public LyricLine(string data)
        {
            // 分离时间轴与句子，格式如[00:00.000] 作词 : じっぷす\n[00:00.338] 作曲 : じっぷす\n
            var lyric = data.Split(new[] { ']' }, 2);
            var time = lyric[0].TrimStart('[').Split(':');
            Time = double.Parse(time[0]) * 60 + double.Parse(time[1]);
            Text = lyric.Length > 1 ? lyric[1].Trim() : string.Empty;
        }

        public LyricLine(double time, string text)
        {
            Time = time;
            Text = text;
        }
    }
}