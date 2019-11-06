using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deploytool.Object
{
    class IPDISTRIBUTION
    {
        public string device { get; set; }
        public string ip { get; set; }
        public string mac { get; set; }
        public IPDISTRIBUTION(string line)
        {

            if (line.Length > 2)
            {
                string[] item = line.Split(',');
                device = item[0];
                ip = item[1];
                if (item.Length == 3 && item[2].Length > 2)
                {
                    mac = item[2];
                }
            }
        }

    }
}
