
#if !IsCore
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace escript
{
    class Aero
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {

            public int cxLeftWidth;

            public int cxRightWidth;

            public int cyTopHeight;

            public int cyBottomHeight;
        }

        public static Margins GetMargins(IntPtr windowHandle, int left, int right, int top, int bottom)
        {
            System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(windowHandle);
            float DesktopDpiX = desktop.DpiX;
            float DesktopDpiY = desktop.DpiY;
            Margins margins = new Margins();
            margins.cxLeftWidth = Convert.ToInt32((left
                            * (DesktopDpiX / 96)));
            margins.cxRightWidth = Convert.ToInt32((right
                            * (DesktopDpiX / 96)));
            margins.cyTopHeight = Convert.ToInt32((top
                            * (DesktopDpiX / 96)));
            margins.cyBottomHeight = Convert.ToInt32((right
                            * (DesktopDpiX / 96)));
            return margins;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct DwmBlurbehind
        {
            public CoreNativeMethods.DwmBlurBehindDwFlags dwFlags;
            public bool Enabled;
            public IntPtr BlurRegion;
            public bool TransitionOnMaximized;
        }

        public static class CoreNativeMethods
        {
            public enum DwmBlurBehindDwFlags
            {
                DwmBbEnable = 1,
                DwmBbBlurRegion = 2,
                DwmBbTransitionOnMaximized = 4
            }
        }

        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
       IntPtr hwnd,
       ref Margins pMarInset);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DwmBlurbehind blurBehind);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);


        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        public static bool Fine()
        {
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                if (DwmIsCompositionEnabled()) return true;
            }
            return false;
        }
        public static void Glass(IntPtr windowHandle, int left = -1, int right = -1, int top = -1, int bottom = -1)
        {
            //WindowInteropHelper windowInterop = new WindowInteropHelper(win);
            //IntPtr windowHandle = windowInterop.Handle;
            //HwndSource mainWindowSrc = HwndSource.FromHwnd(windowHandle);
            //mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;
            if (Fine())
            {
                Margins margins = GetMargins(windowHandle, left, right, top, bottom);
                int returnVal = DwmExtendFrameIntoClientArea(windowHandle, ref margins);
                if ((returnVal < 0))
                {
                    throw new NotSupportedException("?5@0F8O 7025@H8;0AL =5C40G=>");
                }
                else
                {
                    const int dwmwaNcrenderingPolicy = 2;
                    var dwmncrpDisabled = 2;

                    DwmSetWindowAttribute(windowHandle, dwmwaNcrenderingPolicy, ref dwmncrpDisabled, sizeof(int));
                }
            }
        }
    }
}

#endif