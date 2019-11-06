using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Deploytool.lib
{
    class license
    {
        public static int  check()
        {
            int ret;
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream fs = new FileStream("license.dat", FileMode.Open);
                var valid = (DateTime)formatter.Deserialize(fs);
                fs.Close();
                DateTime today = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                ret = today.CompareTo(valid);
            }
            catch
            {
                ret = 2;
            }
            return ret;
        }

    }
}
