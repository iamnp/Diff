using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Diff
{
    internal class CueTextBox : TextBox
    {
        private string _cue;

        public string CueText
        {
            get { return _cue; }
            set
            {
                _cue = value;
                SendMessage(Handle, 0x1501, 0, _cue);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam,
            [MarshalAs(UnmanagedType.LPWStr)] string lParam);
    }
}