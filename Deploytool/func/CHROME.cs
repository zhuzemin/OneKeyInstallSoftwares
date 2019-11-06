using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.IO;

namespace Deploytool.func
{
    class CHROME
    {
        FILEUTILS fileutils;
        public string version { get; set; }
        public string SoftName { get; set; }

        public CHROME(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            //search "Softs" floder for soft path, if exist multiple version, select the newer one
            SoftName = "*chrome*.exe";
            string[] files = fileutils.searchFile(fileutils.path, SoftName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
            SoftName = version.Split('\\').Last();
        }
        public void install()
        {
            string batch = "start \"\" \"" + version + "\"  /silent /install";
            cmd.ExecuteCommand(batch);
        }
    }
}
