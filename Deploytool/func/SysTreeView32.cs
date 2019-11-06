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
using System.Runtime.InteropServices;

namespace Deploytool.func
{
    class SysTreeView32
    {
        //Find Window API
        [DllImport("User32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        //Find WindowEx API
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        //Send Message API
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);

        public const int TV_FIRST = 0x1100;
        public const int TVM_GETCOUNT = TV_FIRST + 5;
        public const int TVM_GETNEXTITEM = TV_FIRST + 10;
        public const int TVM_GETITEMA = TV_FIRST + 12;
        public const int TVM_GETITEMW = TV_FIRST + 62;

        public const int TVGN_ROOT = 0x0000;
        public const int TVGN_NEXT = 0x0001;
        public const int TVGN_PREVIOUS = 0x0002;
        public const int TVGN_PARENT = 0x0003;
        public const int TVGN_CHILD = 0x0004;
        public const int TVGN_FIRSTVISIBLE = 0x0005;
        public const int TVGN_NEXTVISIBLE = 0x0006;
        public const int TVGN_PREVIOUSVISIBLE = 0x0007;
        public const int TVGN_DROPHILITE = 0x0008;
        public const int TVGN_CARET = 0x0009;
        public const int TVGN_LASTVISIBLE = 0x000A;

        public const int TVIF_TEXT = 0x0001;
        public const int TVIF_IMAGE = 0x0002;
        public const int TVIF_PARAM = 0x0004;
        public const int TVIF_STATE = 0x0008;
        public const int TVIF_HANDLE = 0x0010;
        public const int TVIF_SELECTEDIMAGE = 0x0020;
        public const int TVIF_CHILDREN = 0x0040;
        public const int TVIF_INTEGRAL = 0x0080;

        [DllImport("User32.DLL")]
        public static extern int SendMessage(IntPtr hWnd,
            uint Msg, int wParam, int lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess,
            bool bInheritHandle, uint dwProcessId);
        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RELEASE = 0x8000;

        public const uint MEM_RESERVE = 0x2000;
        public const uint PAGE_READWRITE = 4;

        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd,
            out uint dwProcessId);

        [StructLayout(LayoutKind.Sequential)]
        public struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
            public IntPtr HTreeItem;
        }

        public static uint TreeView_GetCount(IntPtr hwnd)
        {
            return (uint)SendMessage(hwnd, TVM_GETCOUNT, 0, 0);
        }

        public static IntPtr TreeView_GetNextItem(IntPtr hwnd, IntPtr hitem, int code)
        {
            return (IntPtr)SendMessage(hwnd, TVM_GETNEXTITEM, code, (int)hitem);
        }

        public static IntPtr TreeView_GetRoot(IntPtr hwnd)
        {
            return TreeView_GetNextItem(hwnd, IntPtr.Zero, TVGN_ROOT);
        }

        public static IntPtr TreeView_GetChild(IntPtr hwnd, IntPtr hitem)
        {
            return TreeView_GetNextItem(hwnd, hitem, TVGN_CHILD);
        }

        public static IntPtr TreeView_GetNextSibling(IntPtr hwnd, IntPtr hitem)
        {
            return TreeView_GetNextItem(hwnd, hitem, TVGN_NEXT);
        }

        public static IntPtr TreeView_GetParent(IntPtr hwnd, IntPtr hitem)
        {
            return TreeView_GetNextItem(hwnd, hitem, TVGN_PARENT);
        }

        public static IntPtr TreeNodeGetNext(IntPtr AHandle, IntPtr ATreeItem)
        {
            if (AHandle == IntPtr.Zero || ATreeItem == IntPtr.Zero) return IntPtr.Zero;
            IntPtr result = TreeView_GetChild(AHandle, ATreeItem);
            if (result == IntPtr.Zero)
                result = TreeView_GetNextSibling(AHandle, ATreeItem);

            IntPtr vParentID = ATreeItem;
            while (result == IntPtr.Zero && vParentID != IntPtr.Zero)
            {
                vParentID = TreeView_GetParent(AHandle, vParentID);
                result = TreeView_GetNextSibling(AHandle, vParentID);
            }
            return result;
        }

        public static bool GetTreeViewText(IntPtr AHandle, List<string> AOutput)
        {
            if (AOutput == null) return false;
            uint vProcessId;
            GetWindowThreadProcessId(AHandle, out vProcessId);

            IntPtr vProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ |
                PROCESS_VM_WRITE, false, vProcessId);
            IntPtr vPointer = VirtualAllocEx(vProcess, IntPtr.Zero, 4096,
                MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            try
            {
                uint vItemCount = TreeView_GetCount(AHandle);
                IntPtr vTreeItem = TreeView_GetRoot(AHandle);
                Console.WriteLine(vItemCount);
                for (int i = 0; i < vItemCount; i++)
                {
                    byte[] vBuffer = new byte[256];
                    TVITEM[] vItem = new TVITEM[1];
                    vItem[0] = new TVITEM();
                    vItem[0].mask = TVIF_TEXT;
                    vItem[0].hItem = vTreeItem;
                    vItem[0].pszText = (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(TVITEM)));
                    vItem[0].cchTextMax = vBuffer.Length;
                    uint vNumberOfBytesRead = 0;
                    WriteProcessMemory(vProcess, vPointer,
                        Marshal.UnsafeAddrOfPinnedArrayElement(vItem, 0),
                        Marshal.SizeOf(typeof(TVITEM)), ref vNumberOfBytesRead);
                    SendMessage(AHandle, TVM_GETITEMA, 0, (int)vPointer);
                    ReadProcessMemory(vProcess,
                        (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(TVITEM))),
                        Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0),
                        vBuffer.Length, ref vNumberOfBytesRead);
                    Console.WriteLine(Marshal.PtrToStringAnsi(
                        Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0)));

                    vTreeItem = TreeNodeGetNext(AHandle, vTreeItem);
                }
            }
            finally
            {
                VirtualFreeEx(vProcess, vPointer, 0, MEM_RELEASE);
                CloseHandle(vProcess);
            }
            return true;
        }

        static public void Start()
        {
            //Handle variables
            int hwnd = 0;
            int treeItem = 0;
            IntPtr hwndChild = IntPtr.Zero;
            IntPtr treeChild = IntPtr.Zero;
            hwnd = FindWindow(null, "Local Group Policy Editor"); //Handle for the application to be controlled
            if (hwnd > 0)
            {
                //Select TreeView Item
                treeChild = FindWindowEx((IntPtr)hwnd, IntPtr.Zero, "MMCViewWindow", null);
                //treeChild = FindWindowEx((IntPtr)treeChild, IntPtr.Zero, "AfxMDIFrame80", null);
                treeChild = FindWindowEx((IntPtr)treeChild, IntPtr.Zero, "SysTreeView32", null);
                if (treeChild != IntPtr.Zero)
                {
                    Console.Write("here");
                    List<string> vOutput = new List<string>();
                    GetTreeViewText((IntPtr)treeChild, vOutput); // xxxx替换成相应treeview句柄。获得参考上贴FindWindow()的使用 
                    foreach (string vLine in vOutput)
                        Console.WriteLine(vLine);
                }
            }
        }
    }
}
