using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deploytool.Object
{
    [Serializable]
    public class SHORTCUT
    {
        public string name { get; set; }
        public string target { get; set; }
        public string path { get; set; }
        public string type { get; set; }

    }
}
