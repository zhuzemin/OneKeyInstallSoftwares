using System;
using System.Management;
using System.Net;
using System.Web.Security;
using System.Windows.Forms;

namespace Deploytool.lib
{
    // Token: 0x02000070 RID: 112
    public class HardwareInfo
    {
        // Token: 0x06000325 RID: 805 RVA: 0x00018E84 File Offset: 0x00017084
        public string GetHostName()
        {
            return Dns.GetHostName();
        }

        // Token: 0x06000326 RID: 806 RVA: 0x00018E98 File Offset: 0x00017098
        public static string CpuGid()
        {
            string text = HardwareInfo.GetHardDiskID().ToLower();
            try
            {
                text = HardwareInfo.GetHardDiskID().ToLower();
            }
            catch (Exception)
            {
                text = HardwareInfo.GetCpuID().ToLower();
            }
            text = HardwareInfo.JiaMi(text);
            return text;
        }

        // Token: 0x06000327 RID: 807 RVA: 0x00018EE8 File Offset: 0x000170E8
        public static string JiaMi(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }

        // Token: 0x06000328 RID: 808 RVA: 0x00018F04 File Offset: 0x00017104
        public static string GetCpuID()
        {
            string result;
            try
            {
                ManagementClass managementClass = new ManagementClass("Win32_Processor");
                ManagementObjectCollection instances = managementClass.GetInstances();
                string text = null;
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        ManagementObject managementObject = (ManagementObject)enumerator.Current;
                        text = managementObject.Properties["ProcessorId"].Value.ToString();
                    }
                }
                result = text;
            }
            catch
            {
                result = "";
            }
            return result;
        }

        // Token: 0x06000329 RID: 809 RVA: 0x00018F98 File Offset: 0x00017198
        public string GetHdId()
        {
            ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher
            {
                Query = new SelectQuery("Win32_DiskDrive", "", new string[]
                {
                    "PNPDeviceID",
                    "signature"
                })
            }.Get();
            ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator();
            enumerator.MoveNext();
            ManagementBaseObject managementBaseObject = enumerator.Current;
            return managementBaseObject.Properties["signature"].Value.ToString().Trim();
        }

        // Token: 0x0600032A RID: 810 RVA: 0x0001901C File Offset: 0x0001721C
        public string getHddInfo()
        {
            ManagementClass managementClass = new ManagementClass("Win32_PhysicalMedia");
            ManagementObjectCollection instances = managementClass.GetInstances();
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    ManagementObject managementObject = (ManagementObject)enumerator.Current;
                    return managementObject.Properties["SerialNumber"].Value.ToString().Trim();
                }
            }
            return "";
        }

        // Token: 0x0600032B RID: 811 RVA: 0x000190A0 File Offset: 0x000172A0
        public string GetBois()
        {
            SelectQuery query = new SelectQuery("SELECT * FROM Win32_BaseBoard");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator();
            enumerator.MoveNext();
            ManagementBaseObject managementBaseObject = enumerator.Current;
            return managementBaseObject.GetPropertyValue("SerialNumber").ToString();
        }

        // Token: 0x0600032C RID: 812 RVA: 0x000190F0 File Offset: 0x000172F0
        public static string GetHardDiskID()
        {
            string result;
            try
            {
                string str = Application.StartupPath.Substring(0, Application.StartupPath.IndexOf(':'));
                new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"" + str + ":\"");
                string text = managementObject.Properties["VolumeSerialNumber"].Value.ToString();
                result = text.Trim();
            }
            catch
            {
                result = "";
            }
            return result;
        }

        // Token: 0x0600032D RID: 813 RVA: 0x0001917C File Offset: 0x0001737C
        public string GetMacAddress()
        {
            string result;
            try
            {
                string text = "";
                ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection instances = managementClass.GetInstances();
                foreach (ManagementBaseObject managementBaseObject in instances)
                {
                    ManagementObject managementObject = (ManagementObject)managementBaseObject;
                    if ((bool)managementObject["IPEnabled"])
                    {
                        text = managementObject["MacAddress"].ToString().Replace(":", "-");
                        break;
                    }
                }
                result = text;
            }
            catch
            {
                result = "unknow";
            }
            finally
            {
            }
            return result;
        }

    }
}
