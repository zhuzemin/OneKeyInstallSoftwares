using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Deploytool.lib;
using Deploytool.Object;
using System.IO;
using System.Windows.Forms;

namespace Deploytool.lib
{
    class REMOVEDRIVE
    {
        //search "Softs" floder for soft path, if exist multiple version, select the newer one
        public FILEUTILS fileutils;
        public string version { get; set; }
        public REMOVEDRIVE(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            string softName = "RemoveDrive.exe";
            string[] files = fileutils.searchFile(fileutils.path, softName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
        }
        public void eject(string disk)
        {
            string batch = "start \"\" \"" + version + "\" " + disk + " -L";
            cmd.ExecuteCommand(batch);
            Console.WriteLine("现在可以拔出U盘.");
        }
    }
}