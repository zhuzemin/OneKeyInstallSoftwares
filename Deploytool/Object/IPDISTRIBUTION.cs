using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Deploytool.Object
{
    class IPDISTRIBUTION
    {
        public string area { get; set; }
        public string device { get; set; }
        public string ip { get; set; }
        public string mac { get; set; }
        public string SN { get; set; }
        public IPDISTRIBUTION(string line)
        {

            string[] item = Regex.Split(line,@",|\t|\s");
            if (item.Length >= 2)
            {
                area= item[0];
                device = item[1];
                ip = item[2];
                if (item.Length >= 3 && item[3].Length > 2)
                {
                    mac = item[3];
                }
                if (item.Length >= 4 && item[4].Length > 2)
                {
                    SN = item[4];
                }
            }
        }

    }
}
