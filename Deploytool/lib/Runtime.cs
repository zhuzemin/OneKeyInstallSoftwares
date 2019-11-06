using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deploytool.lib
{
    class Runtime
    {
        public FILEUTILS fileutils;
        public string SoftPath { get; set; }
        public string SoftName { get; set; }
        public string SoftFolder { get; set; }



        public Runtime(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            SoftName = "VC++2015.exe";
                string[] files = fileutils.searchFile(fileutils.path, SoftName);
                SoftPath = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
                SoftFolder = System.IO.Path.GetDirectoryName(SoftPath);
        }



        public void install()
        {
            string batch = "start \"\" \"" + SoftPath + "\" /VERYSILENT";
            cmd.ExecuteCommand(batch);
        }
    }
}
