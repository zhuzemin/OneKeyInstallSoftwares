using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.IO;

namespace Deploytool.func
{
    class OFFICE
    {
        FILEUTILS fileutils;
        public string version { get; set; }
        public string toolkit { get; set; }
        string toolkitDir;
        string officeDir;
        public string SoftName { get; set; }

        public OFFICE(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            //search "Softs" floder for soft path, if exist multiple version, select the newer one
            string softName = "Setup.exe";
            string second_keyword = "office";
            string[] files = fileutils.searchFile(fileutils.OfficeFolderName, softName, second_keyword);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
            officeDir = System.IO.Path.GetDirectoryName(version);
            //office crack tool
            SoftName = "Office 2010 Toolkit.exe";
            files = fileutils.searchFile(fileutils.path, SoftName);
            toolkit = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
            toolkitDir = System.IO.Path.GetDirectoryName(toolkit);
            SoftName = version.Split('\\').Last();
        }
        public void install()
        {
            string batch = "start /W \"\" \"" + version + "\" /adminfile \"" + toolkitDir + "\\silent install.MSP" + "\" && start /W \"\" \"" + toolkit + "\" /AutoKMS && start /W \"\" \"" + toolkit + "\" /EZ-Activator";
            cmd.ExecuteCommand(batch);
        }
    }
}
