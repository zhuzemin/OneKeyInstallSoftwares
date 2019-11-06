using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.Object;
using System.IO;

namespace Deploytool.lib
{
    class ShortcutManager
    {

        public FILEUTILS fileutils;
        public ShortcutManager(FILEUTILS fileutils){
             this.fileutils = fileutils;
       }
        public List<SHORTCUT> GetDesktopShortcut()
        {
            List<SHORTCUT> shortcuts = new List<SHORTCUT>();
            string[] desktops={
                                  System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                                  System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                              };
           foreach (string path in desktops)
            {
                string[] lnks=fileutils.searchFile(path,"*.lnk");
                foreach(string lnk in lnks){
                    SHORTCUT item = new SHORTCUT();
                    item.name=Path.GetFileName(lnk);
                    item.target = fileutils.GetShortcutTargetFile(lnk);
                    item.path = lnk;
                    shortcuts.Add(item);
                }
            }
            return shortcuts;
        }
        public List<SHORTCUT> ListCompare(List<SHORTCUT> source, List<SHORTCUT> target)
        {
            List<SHORTCUT> TempList = new List<SHORTCUT>();

            foreach (SHORTCUT u1 in source)
            {
                bool duplicatefound = false;
                foreach (SHORTCUT u2 in target)
                    if (u1.name == u2.name)
                    {
                        duplicatefound = true;
                        break;
                    }

                if (!duplicatefound)
                    TempList.Add(u1);
            }
            return TempList;
        }
        public List<SHORTCUT> GetStartMenu()
        {
            List<SHORTCUT> shortcuts = new List<SHORTCUT>();
            string[] startmenus ={
                                  System.Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)+@"\Programs",
                                  System.Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)+@"\Programs"
                              };
            foreach (string path in startmenus)
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                DirectoryInfo[] directories = directory.GetDirectories();
                   String[] sysMenu = { "Accessories", "Administrative Tools", "Maintenance", "Startup", "Games" };
                   String[] sysLnk = {"Media Center.lnk","Sidebar.lnk","Windows DVD Maker.lnk","Windows Fax and Scan.lnk","Windows Media Player.lnk","XPS Viewer.lnk","Internet Explorer.lnk","Internet Explorer(64 bit).lnk"};
                foreach (DirectoryInfo folder in directories)
                {
                    if (!sysMenu.Contains(folder.Name))
                    {
                      SHORTCUT item = new SHORTCUT();
                       item.name = folder.Name;
                       item.path = folder.FullName;
                        shortcuts.Add(item);
                    }
                }
                string[] lnks = fileutils.searchFile(path, "*.lnk",null,false);
                foreach (string lnk in lnks)
                {
                    SHORTCUT item = new SHORTCUT();
                    item.name = Path.GetFileName(lnk);
                    item.target = fileutils.GetShortcutTargetFile(lnk);
                    if (!sysLnk.Contains(item.name))
                    {
                        shortcuts.Add(item);
                    }
                }
            }
            return shortcuts;
        }
    }
}
