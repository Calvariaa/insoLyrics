namespace insoLyrics.Tests.Lyrics
{
    [TestClass]
    public class LyricsFormatTest
    {
        [TestMethod]
         public void FormatTest()
        {
            var li = "[00:00.000] 恬簡 : じっぷす\\n[00:00.338] 恬爆 : じっぷす\\n[00:00.677] ごめんなさい ごめんなさい\\n[00:02.630] その匯冱すら���Yには�oくて\\n[00:04.593] ��えてよ ��えてよ\\n[00:06.782] 書すぐにそこから肇ってしまってよ\\n[00:09.296] 採�rだって暴だけの蒙�悗世�\\n"
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
            var meta = new SearchMetadata("腎�歔ρ�富溺と富定の��", "はな");
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