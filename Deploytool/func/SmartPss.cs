using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Deploytool.lib;

namespace Deploytool.func
{
    class SmartPss
    {
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }
        [DllImport("User32.dll")]
        static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT p); 
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
        

        private string m_classname; // 要找的控件class name 
        private string m_caption;//要找的控件标题 
        private IntPtr m_hWnd; // 找到的控件句柄
        
        public void confirm()
        {
            /*
             * ProcessStartInfo startInfo = new ProcessStartInfo(@"::{208D2C60-3AEA-1069-A2D7-08002B30309D}\::{7007ACC7-3202-11D1-AAD2-00805FC1270E}\::" + adapter.guid);
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
            Thread.Sleep(1000);
             * */
            //IntPtr ArrtibuteHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, adapter.interfaceName + " Properties");
            IntPtr ArrtibuteHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Local Group Policy Editor");
            const int BM_CLICK = 0xF5;
            if (ArrtibuteHwnd != IntPtr.Zero)
            {
                Console.WriteLine("找到窗口");

                //ClickOnPointTool.ClickOnPoint(ArrtibuteHwnd, new System.Drawing.Point(50, 400));
                Thread.Sleep(1000);
                //ClickOnPointTool.ClickOnPoint(ArrtibuteHwnd, new System.Drawing.Point(130, 110));
                Thread.Sleep(1000);
                //IntPtr TabHwnd = ChildWindowFromPoint(ArrtibuteHwnd, new System.Drawing.Point(700, 350));   //获得按钮的句柄  
                IntPtr TabHwnd = FindWindowEx(ArrtibuteHwnd, IntPtr.Zero, "MMCViewWindow", null);   //获得按钮的句柄  
                if (TabHwnd != IntPtr.Zero)
                {
                    Console.WriteLine("找到子窗口");
                    //ClickOnPointTool.ClickOnPoint(TabHwnd, new System.Drawing.Point(170, 120));
                    for (int x = 0; x < 300; x = +10)
                    {
                        for (int y = 0; y < 170; y = +10)
                        {
                            //Console.WriteLine("x:" + x + "," + "y:" + y);
                            //ClickOnPointTool.ClickOnPoint(TabHwnd, new System.Drawing.Point(x, y));
                        }
                    }
                 
                    IntPtr btnHwnd = FindWindowEx(TabHwnd, IntPtr.Zero, "SysTreeView32", null);   //获得按钮的句柄  
                    
                     // IntPtr btnHwnd = FindWindowEx(TabHwnd, IntPtr.Zero, null, "配置(&C)...");   //获得按钮的句柄  
                    if (btnHwnd != IntPtr.Zero)
                    {
                        Console.WriteLine("找到按钮");
                        /*SendMessage(btnHwnd, BM_CLICK, IntPtr.Zero, null);
                        Thread.Sleep(1000);
                        IntPtr PropertiesHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, adapter.device + " 属性");
                        if (PropertiesHwnd != IntPtr.Zero)
                        {
                            //Console.WriteLine("找到按钮");
                            ClickOnPointTool.ClickOnPoint(PropertiesHwnd, new System.Drawing.Point(100, 20));
                        }*/
                    }
                     
                }
            }
        }
    }
}
