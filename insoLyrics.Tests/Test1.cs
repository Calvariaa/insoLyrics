using Microsoft.Win32;
using System.Diagnostics;

namespace insoLyrics.Tests.Lyrics.FromNetease
{
    [TestClass]
    public class Test1
    {
        private static Process _process;
        [TestMethod]
        public void testProccess()
        {
            _process = Process.GetProcessesByName("osu!").FirstOrDefault();
            Console.WriteLine(_process);
            return;
            if (Registry.GetValue(@"HKEY_CLASSES_ROOT\osu!\shell\open\command", null, null) is string exec) {

                Console.WriteLine(exec.Split('"')[1]);
                _process = Process.Start(exec.Split('"')[1]);
                Console.WriteLine(_process);
            }

        }
    } 
}