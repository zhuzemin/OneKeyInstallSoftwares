using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.ServiceProcess;

namespace Deploytool.func
{
    class firewall
    {
        public static void disable()
        {
            //Windows Firewall service
            string MpsSvc = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\MpsSvc";
            /*Disable fw svc 
             * Automatic - 2
             * Manual - 3
             * Disabled - 4
             * Automatic (Delayed Start) - 2 */
            Registry.SetValue(MpsSvc, "Start", 4);
            //get all services
            //var services = ServiceController.GetServices(Environment.MachineName);
            //Windows Firewall service
            //var service = services.First(s => s.ServiceName == "MpsSvc");
            //Stop fw svc
            //service.Stop();
        }
    }
}
