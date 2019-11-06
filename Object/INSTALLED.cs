using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Deploytool.Object
{
    [Serializable]
    public class INSTALLED
    {
        public string name { get; set; }
        public string publisher { get; set; }
        public string UninstallString { get; set; }
        public string path { get; set; }
        public string regkey { get; set; }

    }
}
