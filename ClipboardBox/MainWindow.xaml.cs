using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
namespace ClipboardBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MouseHook.Start();
            MouseHook.MouseAction += new EventHandler(Event);
        }


        // Mouse Click Event
        private void Event(object sender, EventArgs e) 
        { /*EvaluateCaretPosition();*/ Show_Window(); }




       

        //private void GetMousePosition()
        //{
        //    var point = System.Windows.Forms.Cursor.Position;
        //    var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
        //    var mouse = transform.Transform(new Point(point.X, point.Y));
        //}



        private void Show_Window()
        {
            var point = System.Windows.Forms.Cursor.Position;
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(new Point(point.X, point.Y));

            window.Left = mouse.X;
            window.Top = mouse.Y;
        }



        // Elements Logic -----------------------------------------------------------------



        private void locate_Click(object sender, RoutedEventArgs e)
        {
            //window.Left = caretPosition.X;
            //window.Top = caretPosition.Y - window.Height;
        }

        private void moveToFocus_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
    }
}










// MOUSE ----------------------------------------------------------------------------------------


public static class MouseHook
{
    public static event EventHandler MouseAction = delegate { };

    public static void Start()
    {
        _hookID = SetHook(_proc);


    }
    public static void stop()
    {
        UnhookWindowsHookEx(_hookID);
    }

    private static LowLevelMouseProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    private static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_MOUSE_LL, proc,
              GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(
      int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
        {
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            MouseAction(null, new EventArgs());
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private const int WH_MOUSE_LL = 14;

    private enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
      LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
      IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

}