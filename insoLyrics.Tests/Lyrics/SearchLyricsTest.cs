namespace insoLyrics.Tests.Lyrics.FromNetease
{
    [TestClass]
    public class SearchLyricsTest
    {
        [TestMethod]
        public void CheckMeta()
        {
            var meta = new SearchMetadata("空気力学少女と少年の詩", "はな");
            var id = meta.GetMetadata();
            Console.WriteLine(id);
        }

        [TestMethod]
        public void CheckLyrics()
        {
            var meta = new SearchMetadata("空気力学少女と少年の詩", "はな");
            var id = meta.GetMetadata();
            Console.WriteLine(id);

            var lrc = new SearchLyrics(id);
            var res = lrc.GetLyrics();
            Console.WriteLine(res);
        }
    }
}