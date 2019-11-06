using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Deploytool.lib;

namespace Deploytool.func
{
    class winupdate
    {
        public static void Start()
        {
            //get all services
            var services = ServiceController.GetServices(Environment.MachineName);
            //Windows Update service
            var service = services.First(s => s.ServiceName == "wuauserv");
            //Start win update svc
            service.Start();
        }


        public static void Stop()
        {
            //get all services
            var services = ServiceController.GetServices(Environment.MachineName);
            //Windows Update service
            var service = services.First(s => s.ServiceName == "wuauserv");
            //Start win update svc
            service.Stop();
        }




        public static void StartType(string type)
        {
            string batch = "sc config wuauserv start= "+type;
            cmd.ExecuteCommand(batch);

        }


        public static void disable()
        {
            string batch = "sc stop wuauserv";
            cmd.ExecuteCommand(batch);
        }
        }
    }
