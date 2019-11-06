using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.IO;
using Microsoft.Win32;

namespace Deploytool.func
{
    class VNC
    {
          FILEUTILS fileutils;
        public string version{ get; set; }
        public string licenseKey { get; set; }
        public string password { get; set; }

        public VNC(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            //search "Softs" floder for soft path, if exist multiple version, select the newer one
            string softName = "*vnc*.exe";
            string[] files = fileutils.searchFile(fileutils.path, softName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
        }
       public  void install()
        {
            licenseKey = "BR43B-6H233-X43PN-Z6RS7-M5ZTA";
            //password = "baidu.com";
            password = "ff4bc6abee82bfe28f3218ec294ea459";
           //only RealVNC "6" or newer version can applicable "Silent install"(/qn")
            string batch = "start \"\" \"" +  version + "\" /qn REBOOT=ReallySuppress LICENSEKEY=" + licenseKey + " ENABLEAUTOUPDATECHECKS=0 ENABLEANALYTICS=0 SC_AUTH=VNCAuth SC_PWDSRV=" + password;
            cmd.ExecuteCommand(batch);
            string realvnc = @"HKEY_LOCAL_MACHINE\SOFTWARE\RealVNC\vncserver";
            Registry.SetValue(realvnc, "Authentication", "VncAuth");
            Registry.SetValue(realvnc, "Password", password);

        }
    }
}
