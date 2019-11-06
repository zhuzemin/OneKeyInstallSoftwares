using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;

namespace Deploytool.lib
{
    class AutoIt
    {
        //search "Softs" floder for soft path, if exist multiple version, select the newer one
        public FILEUTILS fileutils;
        public string version{get;set;}
        public string softName { get; set; }

        public AutoIt(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            softName = "AutoIt3.exe";
            string[] files = fileutils.searchFile(fileutils.path, softName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
        }

        public void Run(string script)
        {
            string batch = "start \"\" \"" + version + "\" \"" + script + "\"";
            cmd.ExecuteCommand(batch);
        }
    }
}
