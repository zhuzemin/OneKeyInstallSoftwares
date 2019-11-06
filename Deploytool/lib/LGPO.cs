using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Deploytool.lib
{
    class LGPO
    {
        //search "Softs" floder for soft path, if exist multiple version, select the newer one
        public FILEUTILS fileutils;
        public string version { get; set; }
        public string softName { get; set; }
        public string Folder { get; set; }

        public LGPO(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            softName = "LGPO.exe";
            string[] files = fileutils.searchFile(fileutils.path, softName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
            Folder = System.IO.Path.GetDirectoryName(version);
        }

        public void Import()
        {
            string batch = "start \"\" \"" + version + "\" /s \"" + Folder + "\\GptTmpl.inf\"";
            cmd.ExecuteCommand(batch);
        }
    }
}
