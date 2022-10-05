using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace insoLyrics
{
    static class Constants
    {
        public static readonly string _MutexName = "insoLyrics." + Application.ProductVersion;
        //public static readonly string _Path = Application.ExecutablePath + ".cfg";
        public static readonly string _Server = Path.Combine(Path.GetTempPath(), "insoLyrics.dll");
        public static readonly string _BakExt = ".del";
    }
}
