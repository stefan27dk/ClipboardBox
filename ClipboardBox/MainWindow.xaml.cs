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
using System.Windows.Interop;

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
            window.Topmost = true;
        }


        // Mouse Click Event
        private void Event(object sender, EventArgs e) 
        { /*EvaluateCaretPosition();*/ /*Show_Window();*/ }












        // Register HotKey-----------------------::START::------------------------------------------------------------------>  




    


        // Dll import for registering HotKeys

        // Registering HotKeys --------------------------------------------------------------
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        const int MYACTION_HOTKEY_ID = 1; // Hotkey ID


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
            RegisterShortcut();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            if (msg == 0x0312 && wParam.ToInt32() == MYACTION_HOTKEY_ID)
            {
                //// Create only One Form
                //if (Selection_Screenshot == null)
                //{
                //    Selection_Screenshot = new SelectionScreenshot_Form4();
                //}

                //// SHOW Screenshot Form
                //if (Selection_Screenshot.Visible == false)  // If Form is not visible open it
                //{
                //    Selection_Screenshot = new SelectionScreenshot_Form4();
                //    Selection_Screenshot.Show();
                //}
                Show_Window();
            }
                return IntPtr.Zero;
        }


               

        // Register Shortcut - Method
        private void RegisterShortcut()
        {
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;

            // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
            // Compute the addition of each combination of the keys you want to be pressed
            // ALT+CTRL = 1 + 2 = 3 , CTRL+SHIFT = 2 + 4 = 6...
            RegisterHotKey(source.Handle, MYACTION_HOTKEY_ID, 6, (int)Keys.V);
        }

        // Register HotKey-----------------------::END::------------------------------------------------------------------<    











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


    // Global  Mouse Event ------------------------------------------------------------
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