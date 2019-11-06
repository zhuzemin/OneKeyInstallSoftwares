using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.Threading;


namespace Deploytool.func
{
    class FLASH
    {
        FILEUTILS fileutils;
        public string[] files { get; set; }
        public FLASH(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            //search "Softs" floder for soft path, if exist multiple version, select the newer one
            string softName = "*flash*.exe";
            files = fileutils.searchFile(fileutils.path, softName);
            //version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
        }
       public  void install()
        {
            foreach (string file in files)
            {

                string batch = "start /wait \"\" \"" + file + "\" /verysilent /compat IgnoreWarning";
                cmd.ExecuteCommand(batch);
                //Thread.Sleep(3000);
            }
        }
   }
}
