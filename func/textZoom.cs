using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Deploytool.func
{
    class textZoom
    {
        public static void DEFAULT()
        {
            string Desktop = @"HKEY_CURRENT_USER\Control Panel\Desktop";
            /*Windows 7 set text zoom to 100%
             * "96" mean 100%
             * "192" mean 200%
             *need relog*/
            Registry.SetValue(Desktop, "LogPixels", 96);
            //Windows 10 set text zoom to 100%
            Registry.SetValue(Desktop, "Win8DpiScaling", 1);
        }
    }
}
