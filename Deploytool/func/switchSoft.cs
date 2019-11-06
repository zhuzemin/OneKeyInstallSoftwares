using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Deploytool.func
{
    class switchSoft
    {
        //search "Softs" floder for soft path, if exist multiple version, select the newer one
        public FILEUTILS fileutils;
        public string version{get;set;}
        public string softName { get; set; }
        public switchSoft(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            softName = "ShutdownAndMouse.exe";
            string[] files = fileutils.searchFile(fileutils.ShutdownSoftFolderName, softName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
        }
        public void addStartup()
        {
                string ShutdownAndMouse = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run";
            if (DetectSystem.WindowsVersion().Contains("Windows 7"))
            {
                Registry.SetValue(ShutdownAndMouse, "plusbeShutdown", version);
            }
            else if (DetectSystem.WindowsVersion().Contains("Windows 10"))
            {
                Registry.SetValue(ShutdownAndMouse, "plusbeShutdown", version.Replace("ShutdownAndMouse.exe", "StartShowDown.bat"));
                //string batch = "mklink \"%USERPROFILE%\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\StartShowDown.bat\" \"" + version.Replace("ShutdownAndMouse.exe", "StartShowDown.bat") + "\"";
                //cmd.ExecuteCommand(batch);
            }
        }
        public void runAsAdmin()
        {
            string Layers = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";
            Registry.SetValue(Layers,  version, "RUNASADMIN");
        }



        public void Exec()
        {
            string batch = "start  \"\" \"" + version + "\"";
            cmd.ExecuteCommand(batch);
        }

    }
}
