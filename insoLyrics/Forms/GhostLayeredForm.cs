using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static insoLyrics.Interop.NativeMethods;

namespace insoLyrics.Forms
{
    class GhostLayeredForm : LayeredForm
    {
        public GhostLayeredForm()
        {
            ShowInTaskbar = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }
    }
}
