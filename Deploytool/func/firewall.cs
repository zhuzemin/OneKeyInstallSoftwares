using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.ServiceProcess;
using Deploytool.lib;

namespace Deploytool.func
{
    class firewall
    {
        public static void disable()
        {
            string batch = "netsh advfirewall set allprofiles state off";
            cmd.ExecuteCommand(batch);

            //Windows Firewall service
            //string MpsSvc = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\MpsSvc";
            /*Disable fw svc 
             * Automatic - 2
             * Manual - 3
             * Disabled - 4
             * Automatic (Delayed Start) - 2 */
            //Registry.SetValue(MpsSvc, "Start", 4);
            //get all services
            //var services = ServiceController.GetServices(Environment.MachineName);
            //Windows Firewall service
            //var service = services.First(s => s.ServiceName == "MpsSvc");
            //Stop fw svc
            //service.Stop();
        }
        public static void Stop()
        {
            //get all services
            var services = ServiceController.GetServices(Environment.MachineName);
            //Windows Update service
            var service = services.First(s => s.ServiceName == "MpsSvc");
            //Start win update svc
            service.Stop();
        }




        public static void StartType(string type)
        {
            string batch = "sc config MpsSvc start= " + type;
            cmd.ExecuteCommand(batch);

        }
    
}
}
