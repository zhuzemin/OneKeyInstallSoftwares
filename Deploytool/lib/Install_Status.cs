using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.Object;
using Deploytool.func;
using System.Diagnostics;
using System.Threading;

namespace Deploytool.lib
{
    class Install_Status
    {
        public static bool Check(FILEUTILS fileutils, string soft,bool OneTime=false)
        {
            ProgramManager mgr = new ProgramManager(fileutils);
            bool symbol=false;
            do{
                Thread.Sleep(1000);
                if (Process.GetProcessesByName(soft.Replace(".exe","")).FirstOrDefault() == null) symbol = true;
                /*List<INSTALLED> installeds = mgr.installeds();
            symbol = installeds.Any(x => x.name.Contains(soft));*/
                if (OneTime)
                {
                    break;
                }
            }while(!symbol);
            return symbol;
        }
    }
}
