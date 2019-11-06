using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.IO;

namespace Deploytool.func
{

    class KLITE
    {
        FILEUTILS fileutils;
        public string version { get; set; }
        public KLITE(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            //search "Softs" floder for soft path, if exist multiple version, select the newer one
            string softName = "*K-Lite*.exe";
            string[] files = fileutils.searchFile(fileutils.path, softName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
        }
        public void install()
        {
            string batch = "start \"\" \"" + version + "\" /SILENT";
            cmd.ExecuteCommand(batch);
        }
    }
}
