using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Deploytool.lib
{
    class registry
    {
        private RegistryKey detectRootkey(string key){
            string rootkey = key.Split('\\')[0];
            RegistryKey OurKey = null;
            if (rootkey == "HKEY_CLASSES_ROOT" || rootkey == "HKCR")
            {
                OurKey = Registry.ClassesRoot;
            }
            else if (rootkey == "HKEY_CURRENT_USER" || rootkey == "HKUR")
            {
                OurKey = Registry.CurrentUser;
            }
            else if (rootkey == "HKEY_LOCAL_MACHINE" || rootkey == "HKLM")
            {
                OurKey = Registry.LocalMachine;
            }
            else if (rootkey == "HKEY_USERS" || rootkey == "HKU")
            {
                OurKey = Registry.Users;
            }
            else if (rootkey == "HKEY_CURRENT_CONFIG" || rootkey == "HKCC")
            {
                OurKey = Registry.CurrentConfig;
            }
            return OurKey;
        }

        public RegistryKey open(string keypath)
        {
            string rootkey = keypath.Split('\\')[0];
            RegistryKey OurKey = detectRootkey(keypath);
            string subkey = keypath.Replace(rootkey + "\\", "");
            OurKey = OurKey.OpenSubKey(subkey, true);
            return OurKey;
        }
        public RegistryKey find(string keypath,string keyword)
        {
            RegistryKey result=null;
            string rootkey=keypath.Split('\\')[0];
            RegistryKey OurKey=detectRootkey(keypath);
            string subkey = keypath.Replace(rootkey+"\\", "");
            OurKey = OurKey.OpenSubKey(subkey, true);

            foreach (string Keyname in OurKey.GetSubKeyNames())
            {
                RegistryKey unknowkey = OurKey.OpenSubKey(Keyname);
                //find subkey by default value
                if (unknowkey.GetValue(null) != null)
                {
                    if (unknowkey.GetValue(null).ToString() == keyword)
                    {
                        result = unknowkey;
                        break;
                    }
                }
            }
            return result;
        }
        public void deleteKey(string keypath)
        {
            string rootkey = keypath.Split('\\')[0];
            RegistryKey OurKey = detectRootkey(keypath);
            string subkey = keypath.Replace(rootkey + "\\", "");
            OurKey.DeleteSubKey(subkey);
        }
        public void deleteValue(string keypath)
        {
            string value=keypath.Split('\\').Last();
            keypath = keypath.Replace(value, "");
            RegistryKey OurKey = open(keypath);
            OurKey.DeleteValue(value);
        }
    }
}
