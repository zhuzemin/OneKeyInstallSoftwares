using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;

namespace Deploytool.lib
{
    class WinActivation
    {
        public FILEUTILS fileutils;
        public string OemPath { get; set; }
        public string oem = "Oem7F7.exe";
        public string KmsPath { get; set; }
        public string kms = "KMSAuto Net.exe";



        public WinActivation(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            string[] arr = fileutils.searchFile(fileutils.path, oem);
            if(arr.Count()!=0)OemPath = arr[0];
            arr = fileutils.searchFile(fileutils.path, kms);
            if (arr.Count() != 0) KmsPath = arr[0];

        }



        public bool IsWindowsActivated()
        {
            ManagementScope scope = new ManagementScope(@"\\" + System.Environment.MachineName + @"\root\cimv2");
            scope.Connect();

            SelectQuery searchQuery = new SelectQuery("SELECT * FROM SoftwareLicensingProduct WHERE ApplicationID = '55c92734-d682-4d71-983e-d6ec3f16059f' and LicenseStatus = 1");
            ManagementObjectSearcher searcherObj = new ManagementObjectSearcher(scope, searchQuery);

            using (ManagementObjectCollection obj = searcherObj.Get())
            {
                return obj.Count > 0;
            }
        }



        public void Activation()
        {
            if (DetectSystem.WindowsVersion().Contains("Windows 7"))
            {
                System.IO.File.Copy(OemPath, fileutils.target+@"\"+oem, true);
                Console.WriteLine("=================确认所有安装完成后, 退出软件拔掉U盘, 执行"+fileutils.target+"Oem7F7.exe激活Windows.================");
                Process.Start(fileutils.target);

            }
            else if (DetectSystem.WindowsVersion().Contains("Windows 10"))
            {
                string batch = "start /W \"\" \""+KmsPath+"\" /win=act";
                cmd.ExecuteCommand(batch);
                Console.WriteLine("Windows激活完成.");
            }
        }
    }
}
