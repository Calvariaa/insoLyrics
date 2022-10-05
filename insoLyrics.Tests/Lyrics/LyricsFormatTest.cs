namespace insoLyrics.Tests.Lyrics
{
    [TestClass]
    public class LyricsFormatTest
    {
        [TestMethod]
         public void FormatTest()
        {
            var li = "[00:00.000] 作词 : じっぷす\\n[00:00.338] 作曲 : じっぷす\\n[00:00.677] ごめんなさい ごめんなさい\\n[00:02.630] その一言すら脳裏には無くて\\n[00:04.593] 消えてよ 消えてよ\\n[00:06.782] 今すぐにそこから去ってしまってよ\\n[00:09.296] 何時だって私だけの特権だと\\n"
                .Split(new[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var i in li)
            {
                Console.WriteLine(i);
            }


            Lyric lyric = new Lyric(li);
            foreach(var i in lyric)
            {
                Console.WriteLine(i.Time);
                Console.WriteLine(i.Text);
            }
            Assert.IsNotNull(lyric);
        }
        [TestMethod]
        public void FormatFull() 
        {
            var meta = new SearchMetadata("空気力学少女と少年の詩", "はな");
            var id = meta.GetMetadata();
            Console.WriteLine(id);

            var lrc = new SearchLyrics(id);
            var res = lrc.GetLyrics();
            Console.WriteLine(res);

            Lyric lyric = new Lyric(res.Split("\n", StringSplitOptions.RemoveEmptyEntries));
            foreach (var i in lyric)
            {
                Console.WriteLine(i.Time);
                Console.WriteLine(i.Text);
            }
            Assert.IsNotNull(lyric);
        }
    }
}