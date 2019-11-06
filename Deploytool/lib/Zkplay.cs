using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Deploytool.lib
{
    class Zkplay
    {
        public FILEUTILS fileutils;
        public string SoftPath { get; set; }
        public string SoftName { get; set; }
        public string SoftFolder { get; set; }
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        // 取得控件的文字(Text)
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);
        //抓取TextBox的内文会用到
        [DllImport("User32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);

        public Zkplay(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            SoftName = "GUID加密.exe";
            if (Directory.Exists(fileutils.ZkplayFolderName))
            {
                string[] files = fileutils.searchFile(fileutils.ZkplayFolderName, SoftName);
                SoftPath = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
                SoftFolder = System.IO.Path.GetDirectoryName(SoftPath);
                fileutils.ZkplayFolderName = SoftFolder;
            }
        }


        private static SymmetricAlgorithm symmetricAlgorithm_0 = new TripleDESCryptoServiceProvider();

        public static int Level
        {
            get;
            set;
        }

        public static string SN
        {
            get;
            set;
        }

        public static bool IsKey
        {
            get;
            set;
        }
        public static bool GetKey()
        {
            try
            {
                string text = smethod_0();
                if (text == "")
                {
                    return false;
                }
                string sKey = "qJzGVFEGESZDVJeCnFPBNsvnhB7NLQM5";
                string sIV = "VBNGCjeHJLK=";
                text = DecryptString(text, sKey, sIV);
                string[] array = text.Split('|');
                if (array.Length == 3 && array[0] == Deploytool.lib.HardwareInfo.CpuGid())
                {
                    IsKey = true;
                    SN = array[1];
                    Level = Convert.ToInt32(array[2]);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        private static string smethod_0()
        {
            string path = "C://Data//key.ini";
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return "";
        }
        public static string DecryptString(string Value, string sKey, string sIV)
        {
            try
            {
                symmetricAlgorithm_0.Key = Convert.FromBase64String(sKey);
                symmetricAlgorithm_0.IV = Convert.FromBase64String(sIV);
                symmetricAlgorithm_0.Mode = CipherMode.ECB;
                symmetricAlgorithm_0.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = symmetricAlgorithm_0.CreateDecryptor(symmetricAlgorithm_0.Key, symmetricAlgorithm_0.IV);
                byte[] array = Convert.FromBase64String(Value);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
                cryptoStream.Write(array, 0, array.Length);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                return "Error in Decrypting " + ex.Message;
            }
        }
        public static string Encrypt(string GUID)
        {
            string encryptPassword = string.Empty;

            SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
            algorithm.Key = Convert.FromBase64String("qJzGVFEGESZDVJeCnFPBNsvnhB7NLQM5");
            algorithm.IV = Convert.FromBase64String("VBNGCjeHJLK=");
            algorithm.Mode = CipherMode.ECB;
            algorithm.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = algorithm.CreateEncryptor();

            byte[] data = (new System.Text.ASCIIEncoding()).GetBytes(GUID+"|18942329198zfy08308226zybj|2");
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);

            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            encryptPassword = Convert.ToBase64String(memoryStream.ToArray());

            memoryStream.Close();
            cryptoStream.Close();

            return(encryptPassword);
        }



        public string GetGUID(string SoftPath)
        {
            string keyWord = "";
            ProcessStartInfo startInfo = new ProcessStartInfo(SoftPath);
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
            Thread.Sleep(2000);
            IntPtr MainHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "GUID");
            if (MainHwnd != IntPtr.Zero) {
                IntPtr BTNBoxHwnd = FindWindowEx(MainHwnd, IntPtr.Zero, null, "写入");   //获得按钮的句柄  
                IntPtr FirstTextBoxHwnd = FindWindowEx(MainHwnd, BTNBoxHwnd, null, null);   //获得按钮的句柄  
                IntPtr SecondTextBoxHwnd = FindWindowEx(MainHwnd, FirstTextBoxHwnd, null, null);   //获得按钮的句柄  
                if (SecondTextBoxHwnd != IntPtr.Zero)
                {
                    StringBuilder title = new StringBuilder(1024);
                    int len;
                    len = GetWindowText(SecondTextBoxHwnd, title, title.Capacity);

                    if (len > 0)
                    {
                        keyWord += title.ToString() + "\r\n";
                    }
                    else // 可能为TextBox，注意，TextBox用以上方法抓取不到，所以改为以下方法
                    {

                        int WM_GETTEXT = 0x000D;
                        int num = SendMessage(SecondTextBoxHwnd, WM_GETTEXT, title.Capacity, title);
                        if (title.Length > 0)
                            keyWord += title.ToString() + "\r\n";
                    }
                    Thread.Sleep(2000);
                    int WM_CLOSE = 0x10;
                    SendMessage(MainHwnd, WM_CLOSE, 0, null);
                }
        }
            return keyWord.Trim();
        }


        public static string GetGUID_Abandon()
        {
            
            return HardwareInfo.JiaMi(HardwareInfo.GetHardDiskID().ToLower());
        }



        public static void WrtiteKey(string key)
        {
            string text = "C://Data";
            string path = text + "//key.ini";
            string text2 = key.Replace(" ", "");
            if (text2 == "")
            {
                return;
            }
            else
            {
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                    if (!File.Exists(path))
                    {
                        using (StreamWriter streamWriter = File.CreateText(path))
                        {
                            streamWriter.Write(text2);
                        }
                    }
                    using (StreamWriter streamWriter2 = File.CreateText(path))
                    {
                        streamWriter2.Write(text2);
                    }
                }
                if (!File.Exists(path))
                {
                    using (StreamWriter streamWriter3 = File.CreateText(path))
                    {
                        streamWriter3.Write(text2);
                    }
                }
                using (StreamWriter streamWriter4 = File.CreateText(path))
                {
                    streamWriter4.Write(text2);
                }
            }
        }



        public void addStartup()
        {
            string key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run";
            Registry.SetValue(key, "Zkplay8020", fileutils.ZkplayFolderName + @"\Start.bat");
        }

    }
}
