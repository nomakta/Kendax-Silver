using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Sulakore.Habbo.Components
{
    public class HClient : WebBrowser
    {
        private IntPtr _IEHandle;
        private IntPtr IEHandle
        {
            get
            {
                if (_IEHandle != IntPtr.Zero) return _IEHandle;

                StringBuilder BCName = new StringBuilder(100);
                IntPtr _Handle = base.Handle;
                do NativeMethods.GetClassName((_Handle = NativeMethods.GetWindow(_Handle, 5)), BCName, BCName.MaxCapacity);
                while (BCName.ToString() != "Internet Explorer_Server");

                return (_IEHandle = _Handle);
            }
        }

        public HClient()
            : base()
        { }

        new public void Enter()
        {
            NativeMethods.PostMessage(IEHandle, 256u, 13, IntPtr.Zero);
        }
        new public void Click(int X, int Y)
        {
            NativeMethods.SendMessage(IEHandle, 0x201, IntPtr.Zero, (IntPtr)((Y << 16) | X));
            NativeMethods.SendMessage(IEHandle, 0x202, IntPtr.Zero, (IntPtr)((Y << 16) | X));
        }
        new public void Click(Point Coordinate)
        {
            Click(Coordinate.X, Coordinate.Y);
        }

        public void Say(string Message)
        {
            Speak(Message, false);
        }
        public void Shout(string Message)
        {
            Speak(Message, true);
        }
        public void Speak(string Message, bool Shout)
        {
            if (string.IsNullOrEmpty(Message)) return;

            Enter();
            if (Shout) Message = ":shout " + Message;
            foreach (char C in Message) NativeMethods.PostMessage(IEHandle, 0x102, C, IntPtr.Zero);
            Enter();
        }

        public void Sign(HSigns Sign)
        {
            Say(":sign " + Sign.Juice());
        }
        public void Stance(HStances Stance)
        {
            Say(":" + Stance.ToString());
        }
        public void Gesture(HGestures Gesture)
        {
            switch (Gesture)
            {
                case HGestures.Wave: Say("o/"); break;
                case HGestures.Idle: Say(":idle"); break;
                case HGestures.ThumbsUp: Say("_b"); break;
                case HGestures.BlowKiss: Say(":kiss"); break;
                case HGestures.Laugh: Say(":whisper  :D"); break;
            }
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool PostMessage(IntPtr hwnd, uint msg, int wparam, IntPtr lparam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        }
    }
}