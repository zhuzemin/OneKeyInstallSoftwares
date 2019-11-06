using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deploytool.lib
{
    class DetectSystem
    {
        public static string WindowsVersion()
        {
            string subKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey skey = key.OpenSubKey(subKey);
            string name=null;
            try
            {
                name = skey.GetValue("ProductName").ToString();
            }
            catch
            {
            }
            return name;
        }
    }
}
