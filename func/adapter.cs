using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using Microsoft.Win32;
using Deploytool.lib;

namespace Deploytool.func
{
    class ADAPTER
    {
        public ManagementObjectCollection interfaceCollection;
        public string interfaceName { get; set; }
        public string device { get; set; }
        public string mac { get; set; }
        public string index { get; set; }
        public string guid { get; set; }
        public ADAPTER()
        {
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery =
                new ObjectQuery("Select * From Win32_NetworkAdapter");
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oReturnCollection = oSearcher.Get();
            interfaceCollection = oReturnCollection;
        }
        public void setIP(string ip_address, string subnet_mask)
         {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                //for handled exception
                string MAC = "";
                try
                {
                    MAC = objMO["MACAddress"].ToString();
                }
                catch (NullReferenceException err)
                {
                }
                if (MAC.Equals(mac))
                {
                    ManagementBaseObject setIP;
                    ManagementBaseObject newIP =
                      objMO.GetMethodParameters("EnableStatic");

                    newIP["IPAddress"] = new string[] { ip_address };
                    newIP["SubnetMask"] = new string[] { subnet_mask };

                    setIP = objMO.InvokeMethod("EnableStatic", newIP, null);
                }

            }
         }
        public void setGateway(string gateway)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                //for handled exception
                string MAC = "";
                try
                {
                    MAC = objMO["MACAddress"].ToString().Replace(":","-");
                }
                catch (NullReferenceException err)
                {
                }
                if (MAC.Equals(mac))
                {
                    ManagementBaseObject setGateway;
                    ManagementBaseObject newGateway =
                      objMO.GetMethodParameters("SetGateways");

                    newGateway["DefaultIPGateway"] = new string[] { gateway };
                    newGateway["GatewayCostMetric"] = new int[] { 1 };

                    setGateway = objMO.InvokeMethod("SetGateways", newGateway, null);
                    break;
                }
            }
        }

        public void setDNS(string NIC, string DNS)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    // if you are using the System.Net.NetworkInformation.NetworkInterface
                    // you'll need to change this line to
                    // if (objMO["Caption"].ToString().Contains(NIC))
                    // and pass in the Description property instead of the name 
                    if (objMO["Caption"].Equals(NIC))
                    {
                        ManagementBaseObject newDNS =
                          objMO.GetMethodParameters("SetDNSServerSearchOrder");
                        newDNS["DNSServerSearchOrder"] = DNS.Split(',');
                        ManagementBaseObject setDNS =
                          objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                    }
                }
            }
        }

        public void setWINS(string NIC, string priWINS, string secWINS)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    if (objMO["Caption"].Equals(NIC))
                    {
                        ManagementBaseObject setWINS;
                        ManagementBaseObject wins =
                        objMO.GetMethodParameters("SetWINSServer");
                        wins.SetPropertyValue("WINSPrimaryServer", priWINS);
                        wins.SetPropertyValue("WINSSecondaryServer", secWINS);

                        setWINS = objMO.InvokeMethod("SetWINSServer", wins, null);
                    }
                }
            }
        }

        public void allowAwaken()
        {
            string devices = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class";
            var reg = new registry();
            RegistryKey adapters=reg.find(devices,"Network adapters");
            try
            {
                String adapter = adapters.OpenSubKey(index.PadLeft(4, '0')).ToString();
            Registry.SetValue(adapter, "PnPCapabilities", 0);
            }
            catch (Exception err) { }
            /*adapter power management setting, 
             * "0" mean "ALL ON"
             * "8" mean "ALL OFF"
             * "18" mean "for saving power turn device power off" "ON"*/
            //restart adapter for enable
            //disable(interfaceName);
            //enable(interfaceName);
        }
        public void enable(string interfaceName)
        {
                   cmd.ExecuteCommand("netsh interface set interface \"" + interfaceName + "\" enable");
            
        }

        public void disable(string interfaceName)
        {
                cmd.ExecuteCommand("netsh interface set interface \"" + interfaceName + "\" disable");
           
        }
    }
}