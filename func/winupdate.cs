using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace Deploytool.func
{
    class winupdate
    {
        public static void enable()
        {
            //get all services
            var services = ServiceController.GetServices(Environment.MachineName);
            //Windows Update service
            var service = services.First(s => s.ServiceName == "wuauserv");
            //Start win update svc
            service.Start();
        }
    }
}
