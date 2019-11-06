using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using Deploytool.Object;
using Microsoft.Win32;

namespace Deploytool.func
{
    class StartupManager
    {
        public FILEUTILS fileutils;
        public StartupManager(FILEUTILS fileutils){
            this.fileutils = fileutils;
        }
        public List<SHORTCUT> GetStartupList()
        {
            List<SHORTCUT> StartupItems = new List<SHORTCUT>();
            string[] StartupFolders ={
                                  System.Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup),
                                  System.Environment.GetFolderPath(Environment.SpecialFolder.Startup)
                              };
            string[] StartupRegistrys={
                                          @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run",
                                          @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run",
                                          @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Run",
                                      };
            foreach (string path in StartupFolders)
            {
                string[] lnks = fileutils.searchFile(path, "*.lnk",null,false);
                foreach (string lnk in lnks)
                {
                    SHORTCUT item = new SHORTCUT();
                    item.name = System.IO.Path.GetFileName(lnk);
                    item.target = fileutils.GetShortcutTargetFile(lnk);
                    item.type = "lnk";
                    item.path=lnk;
                    StartupItems.Add(item);
                }
            }
            registry regedit=new registry();
            foreach(string reg in StartupRegistrys)
            {
                RegistryKey OurKey = regedit.open(reg);
                foreach (string Keyname in OurKey.GetValueNames())
                {
                    string Keyvalue=OurKey.GetValue(Keyname).ToString();
                    SHORTCUT item = new SHORTCUT();
                    item.name = Keyname;
                    item.target = Keyvalue;
                    item.type = "reg";
                    item.path=OurKey+@"\"+Keyname;
                    StartupItems.Add(item);
                }
            }
            return StartupItems;
        }
        public List<List<SHORTCUT>> ListCompare(List<SHORTCUT> source, List<SHORTCUT> target)
        {
            List<SHORTCUT> NewSource = new List<SHORTCUT>();
            List<SHORTCUT> NewTarget = new List<SHORTCUT>();
            foreach (SHORTCUT u1 in source)
            {
                bool duplicatefound = false;
                foreach (SHORTCUT u2 in target)
                    if (u1.name == u2.name)
                    {
                        duplicatefound = true;
                        NewTarget.Add(u2);
                        break;
                    }

                if (!duplicatefound)
                    NewSource.Add(u1);
            }
            return new List<List<SHORTCUT>>(){NewSource,NewTarget};
        }
    }
}
