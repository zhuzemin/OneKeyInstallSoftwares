using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Deploytool.Object
{
    class CB_STATE
    {
        public Boolean vnc { get; set; }
        public Boolean klite { get; set; }
        public Boolean office { get; set; }
        public Boolean chrome { get; set; }
        public Boolean smb1 { get; set; }
        public ItemCollection uninstall { get; set; }
        public Boolean flash { get; set; }
        public Boolean grouppolicy { get; set; }
        public Boolean activation { get; set; }
        public Boolean runtime { get; set; }
    }
}
