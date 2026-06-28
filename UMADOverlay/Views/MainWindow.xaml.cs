using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using UMADOverlay.ViewModels;

namespace UMADOverlay.Views
{
    public partial class MainWindow : Window
    {
        const int WS_EX_NOACTIVATE = 0x08000000;
        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int GWL_EXSTYLE      = -20;

        [DllImport("user32.dll")] static extern int GetWindowLong(IntPtr h, int n);
        [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr h, int n, int v);

        const int WM_MOUSEACTIVATE = 0x0021;
        const int MA_NOACTIVATE    = 3;
        const int WM_NCHITTEST     = 0x0084;

        // NCHITTEST return values for resize edges
        const int HTLEFT        = 10;
        const int HTRIGHT       = 11;
        const int HTTOP         = 12;
        const int HTTOPLEFT     = 13;
        const int HTTOPRIGHT    = 14;
        const int HTBOTTOM      = 15;
        const int HTBOTTOMLEFT  = 16;
        const int HTBOTTOMRIGHT = 17;
        const int HTCLIENT      = 1;

        const int GRIP = 8; // resize border thickness in pixels

        OverlayViewModel? _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new OverlayViewModel();
            DataContext = _vm;
            _vm.SetWindow(this);

            SourceInitialized += (_, _) =>
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                int ex = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, ex | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
                HwndSource.FromHwnd(hwnd)?.AddHook(WndProc);
            };

            SizeChanged += (_, e) =>
            {
                if (_vm == null || !_vm.SizeUnlocked) return;
                // Sync back to ViewModel without triggering Left compensation
                _vm.SyncSize(e.NewSize.Width, e.NewSize.Height);
            };

            // Minimize: collapse to drag bar height only
            double _savedHeight = _vm.WindowHeight;
            double _savedWidth  = _vm.WindowWidth;
            _vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName != nameof(OverlayViewModel.IsMinimized)) return;
                if (_vm.IsMinimized)
                {
                    _savedHeight = ActualHeight;
                    _savedWidth  = ActualWidth;
                    MinHeight = 32;
                    MaxHeight = 32;
                    Height    = 32;
                }
                else
                {
                    MinHeight = 400;
                    MaxHeight = 1000;
                    Width     = _savedWidth;
                    Height    = _savedHeight;
                    _vm.SyncSize(_savedWidth, _savedHeight);
                }
            };
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return (IntPtr)MA_NOACTIVATE;
            }

            // Custom resize hit-testing when unlocked
            if (msg == WM_NCHITTEST && (_vm?.SizeUnlocked ?? false))
            {
                var pos    = new System.Windows.Point(
                    (short)(lParam.ToInt32() & 0xFFFF),
                    (short)((lParam.ToInt32() >> 16) & 0xFFFF));
                var local  = PointFromScreen(pos);
                double w   = ActualWidth;
                double h   = ActualHeight;
                bool left  = local.X < GRIP;
                bool right = local.X > w - GRIP;
                bool top   = local.Y < GRIP;
                bool bot   = local.Y > h - GRIP;

                int ht = HTCLIENT;
                if (top    && left)  ht = HTTOPLEFT;
                else if (top  && right) ht = HTTOPRIGHT;
                else if (bot  && left)  ht = HTBOTTOMLEFT;
                else if (bot  && right) ht = HTBOTTOMRIGHT;
                else if (top)           ht = HTTOP;
                else if (bot)           ht = HTBOTTOM;
                else if (left)          ht = HTLEFT;
                else if (right)         ht = HTRIGHT;

                if (ht != HTCLIENT)
                {
                    handled = true;
                    return (IntPtr)ht;
                }
            }

            return IntPtr.Zero;
        }

        void DragBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
