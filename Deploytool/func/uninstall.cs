using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Deploytool.lib;
using Deploytool.Object;
using System.IO;
using System.Windows.Forms;

namespace Deploytool.func
{
    class UNINSTALL
    {
        //search "Softs" floder for soft path, if exist multiple version, select the newer one
        public FILEUTILS fileutils;
        public string version {get;set;}
        public string[] excludePublishers;
        public UNINSTALL(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
         string fileName=@"\execludePublisher.txt";
         excludePublishers = File.ReadAllLines(fileutils.path+fileName, System.Text.Encoding.Default);
         string softName = "Unlocker.exe";
            string[] files = fileutils.searchFile(fileutils.path, softName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
        }
        public class ListWithDuplicates : List<KeyValuePair<RegistryKey, string>>
        {
            public void Add(RegistryKey key, string value)
            {
                var element = new KeyValuePair<RegistryKey, string>(key, value);
                this.Add(element);
            }
        }
        public List<INSTALLED> installeds()
        {
            var list = new ListWithDuplicates();
            list.Add(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            list.Add(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            list.Add(Registry.LocalMachine, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            
            List<INSTALLED> installeds = new List<INSTALLED>();
            foreach (var item in list)
            {
                RegistryKey subkey = item.Key.OpenSubKey(item.Value);
                foreach (string installed in subkey.GetSubKeyNames())
                {
                    using (RegistryKey info = subkey.OpenSubKey(installed))
                    {
                        if (info.GetValue("UninstallString") != null && info.GetValue("DisplayName") != null && !excludePublishers.Any(info.GetValue("DisplayName").ToString().Contains))
                        {
                            if (info.GetValue("Publisher") == null || !excludePublishers.Contains(info.GetValue("Publisher").ToString()) )
                            {
                                INSTALLED soft = new INSTALLED();
                                soft.name = info.GetValue("DisplayName").ToString();
                                if (info.GetValue("Publisher") == null)
                                {
                                    soft.publisher = null;
                                }
                                else
                                {
                                    soft.publisher = info.GetValue("Publisher").ToString();
                                }
                                string UninstallString = info.GetValue("UninstallString").ToString();
                                soft.UninstallString = UninstallString;
                                string[] array = UninstallString.Split(new[] { @"\" }, StringSplitOptions.None);
                                string path = UninstallString.Replace(@"\"+array.Last(), "");
                                soft.path = path;
                                soft.regkey = info.Name;

                                installeds.Add(soft);
                            }

                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            return installeds;
        }
        
        public void uninstall(INSTALLED soft)
        {
            string batch = "start \"\" \""+version +  "\"  \"" + soft.path + "\" /D /S";
            cmd.ExecuteCommand(batch);
            var reg = new registry();
            reg.delete(soft.regkey);

         }
    }
}
