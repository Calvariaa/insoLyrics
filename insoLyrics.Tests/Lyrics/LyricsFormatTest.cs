namespace insoLyrics.Tests.Lyrics
{
    [TestClass]
    public class LyricsFormatTest
    {
        [TestMethod]
         public void FormatTest()
        {
            var li = "[00:00.000] ���� : ���äפ�\\n[00:00.338] ���� : ���äפ�\\n[00:00.677] �����ʤ��� �����ʤ���\\n[00:02.630] ����һ�Ԥ���×�Y�ˤϟo����\\n[00:04.593] �����Ƥ� �����Ƥ�\\n[00:06.782] �񤹤��ˤ�������ȥ�äƤ��ޤäƤ�\\n[00:09.296] �Εr���ä�˽�������ؘؤ���\\n"
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
            var meta = new SearchMetadata("�՚���ѧ��Ů�������Ԋ", "�Ϥ�");
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