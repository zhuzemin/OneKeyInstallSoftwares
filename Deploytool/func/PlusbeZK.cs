using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Xml;

namespace Deploytool.func
{
    class PlusbeZK
    {
        public FILEUTILS fileutils;
        public string KeyPath { get; set; }
        public string Key { get; set; }
        public string SN { get; set; }
        public string SetConfigPath { get; set; }
        public string SetConfig { get; set; }
        public string SoftPath { get; set; }
        public string Soft = "PlusbeZk.exe";
        XmlDocument xml = new XmlDocument();
        XmlNode ParentNode;


        public PlusbeZK(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            if (Directory.Exists(fileutils.PlusbeZKFolderName)) { 
            KeyPath = fileutils.searchFile(fileutils.PlusbeZKFolderName, "key.txt")[0];
            //SN= HardJM.GetSN();
            //Key= HardJM.GetKey();
            SetConfigPath = fileutils.searchFile(fileutils.PlusbeZKFolderName, "SetConfig.xml")[0];
            SetConfig=File.ReadAllText(SetConfigPath, System.Text.Encoding.Default);
            SoftPath = fileutils.searchFile(fileutils.PlusbeZKFolderName, Soft)[0];
            XmlNodeList nodeList;
            xml.Load(SetConfigPath);
            nodeList = xml.GetElementsByTagName("SetConfig");
            ParentNode = nodeList[0];
            }
        }



        public void Reg()
        {
            System.IO.File.WriteAllText(KeyPath, Key);
        }



        public void ContentServer(string server, string identity,string interval)
        {
            ParentNode.SelectSingleNode("serverIP").InnerText=server;
            ParentNode.SelectSingleNode("myIp").InnerText = identity;
            ParentNode.SelectSingleNode("serverTime").InnerText = interval;
            ParentNode.SelectSingleNode("serverMode").InnerText = "1";
            ParentNode.SelectSingleNode("Replay").InnerText = "true";
        }



        public void Resolution(string screenWidth,string screenHeight)
        {
            ParentNode.SelectSingleNode("Height").InnerText = screenHeight;
            ParentNode.SelectSingleNode("Width").InnerText = screenWidth;
            ParentNode.SelectSingleNode("Max").InnerText = "2";
        }



        public void TopMost()
        {
            ParentNode.SelectSingleNode("TopMost").InnerText = "1";
        }



        public void LoopMode(string mode)
        {
            ParentNode.SelectSingleNode("movieAutoPlay").InnerText = mode;
        }



        public void HideCursor()
        {
            ParentNode.SelectSingleNode("HideCursor").InnerText = "1";
        }



        public void AutoPlay()
        {
            ParentNode.SelectSingleNode("bPlay").InnerText = "true";
        }



        public void runAsAdmin()
        {
            string Layers = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers";
            Registry.SetValue(Layers, SoftPath, "RUNASADMIN");
        }



        public void addStartup()
        {
            string key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run";
            Registry.SetValue(key, "PlusbeZK8020", SoftPath.Replace(".exe", ".bat"));
        }



        public void Save()
        {
            xml.Save(SetConfigPath);
        }

    }
}
