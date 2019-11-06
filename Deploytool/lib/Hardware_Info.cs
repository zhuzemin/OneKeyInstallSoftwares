using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Deploytool.lib
{
    class Hardware_Info
    {
        public static void DeviceDetail()
        {
            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                Console.WriteLine("本机CPU信息如下:");
                Console.WriteLine("Name  -  " + obj["Name"]);
                /*Console.WriteLine("DeviceID  -  " + obj["DeviceID"]);
                Console.WriteLine("Manufacturer  -  " + obj["Manufacturer"]);
                Console.WriteLine("CurrentClockSpeed  -  " + obj["CurrentClockSpeed"]);
                Console.WriteLine("Caption  -  " + obj["Caption"]);
                Console.WriteLine("NumberOfCores  -  " + obj["NumberOfCores"]);
                Console.WriteLine("NumberOfEnabledCore  -  " + obj["NumberOfEnabledCore"]);
                Console.WriteLine("NumberOfLogicalProcessors  -  " + obj["NumberOfLogicalProcessors"]);
                Console.WriteLine("Architecture  -  " + obj["Architecture"]);
                Console.WriteLine("Family  -  " + obj["Family"]);
                Console.WriteLine("ProcessorType  -  " + obj["ProcessorType"]);
                Console.WriteLine("Characteristics  -  " + obj["Characteristics"]);
                Console.WriteLine("AddressWidth  -  " + obj["AddressWidth"]);
                 * */
            }

            foreach (ManagementObject obj in myVideoObject.Get())
            {
                Console.WriteLine("本机显卡信息如下:");
                Console.WriteLine("Name  -  " + obj["Name"]);
                /*
                 * Console.WriteLine("Status  -  " + obj["Status"]);
                Console.WriteLine("Caption  -  " + obj["Caption"]);
                Console.WriteLine("DeviceID  -  " + obj["DeviceID"]);
                Console.WriteLine("AdapterRAM  -  " + obj["AdapterRAM"]);
                Console.WriteLine("AdapterDACType  -  " + obj["AdapterDACType"]);
                Console.WriteLine("Monochrome  -  " + obj["Monochrome"]);
                Console.WriteLine("InstalledDisplayDrivers  -  " + obj["InstalledDisplayDrivers"]);
                Console.WriteLine("DriverVersion  -  " + obj["DriverVersion"]);
                Console.WriteLine("VideoProcessor  -  " + obj["VideoProcessor"]);
                Console.WriteLine("VideoArchitecture  -  " + obj["VideoArchitecture"]);
                Console.WriteLine("VideoMemoryType  -  " + obj["VideoMemoryType"]);
                 * */
            }
        }
    }
}
