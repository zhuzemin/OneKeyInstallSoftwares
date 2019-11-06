﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Deploytool.lib;
using Deploytool.Object;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Deploytool.func
{
    class ProgramManager
    {
        //search "Softs" floder for soft path, if exist multiple version, select the newer one
        public FILEUTILS fileutils;
        public string[] excludePublishers;
        public ProgramManager(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            excludePublishers = File.ReadAllLines(fileutils.path + @"\excludePublisher.txt", System.Text.Encoding.Default);
        }
        public List<INSTALLED> installeds()
        {
            Tuple<RegistryHive, RegistryView, string>[] list = { 
            Tuple.Create(RegistryHive.CurrentUser,RegistryView.Registry64, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            Tuple.Create(RegistryHive.LocalMachine,RegistryView.Registry64, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            Tuple.Create(RegistryHive.LocalMachine,RegistryView.Registry32, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
                                                             };

            List<INSTALLED> installeds = new List<INSTALLED>();
            foreach (var item in list)
            {
                try
                {
                    RegistryKey subkey = RegistryKey.OpenBaseKey(item.Item1, item.Item2).OpenSubKey(item.Item3);
                    foreach (string installed in subkey.GetSubKeyNames())
                    {
                        using (RegistryKey info = subkey.OpenSubKey(installed))
                        {
                            if (info.GetValue("UninstallString") != null && info.GetValue("DisplayName") != null && !excludePublishers.Any(info.GetValue("DisplayName").ToString().Contains))
                            {
                                if (info.GetValue("Publisher") == null || !excludePublishers.Contains(info.GetValue("Publisher").ToString()))
                                {
                                    INSTALLED soft = new INSTALLED();
                                    soft.name = info.GetValue("DisplayName").ToString();
                                    //Console.WriteLine(soft.name);
                                    if (info.GetValue("Publisher") == null)
                                    {
                                        soft.publisher = null;
                                    }
                                    else
                                    {
                                        soft.publisher = info.GetValue("Publisher").ToString();
                                    }
                                    string UninstallString = info.GetValue("UninstallString").ToString();
                                    soft.UninstallString = UninstallString;
                                    string[] array = UninstallString.Split(new[] { @"\" }, StringSplitOptions.None);
                                    string path = UninstallString.Replace(@"\" + array.Last(), "");
                                    soft.path = path;
                                    soft.regkey = info.Name;

                                    installeds.Add(soft);
                                }

                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            return ListDistinct(installeds);
        }

        public void uninstall(INSTALLED soft)
        {
            string batch;
            if (soft.path.Contains("MsiExec"))
            {
                soft.path = soft.path.Replace("/I", "/X");
                batch = soft.path + " /qn /norestart";
                cmd.ExecuteCommand(batch);
            }
            else
            {
                if (File.Exists(soft.UninstallString))
                {
                ProcessStartInfo startInfo = new ProcessStartInfo(soft.UninstallString);
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                }
                else
                {
                    Console.WriteLine(soft.UninstallString+"不存在."); 
                }
            }
        }
        public List<INSTALLED> ListDistinct(List<INSTALLED> SearchResults)
        {
            List<INSTALLED> TempList = new List<INSTALLED>();

            foreach (INSTALLED u1 in SearchResults)
            {
                bool duplicatefound = false;
                foreach (INSTALLED u2 in TempList)
                    if (u1.name == u2.name)
                    {
                        duplicatefound = true;
                        break;
                    }

                if (!duplicatefound)
                    TempList.Add(u1);
            }
            return TempList;
        }
        public List<List<INSTALLED>> ListCompare(List<INSTALLED> source, List<INSTALLED> target)
        {
            List<INSTALLED> NewSource = new List<INSTALLED>();
            List<INSTALLED> NewTarget = new List<INSTALLED>();
            foreach (INSTALLED u1 in source)
            {
                bool duplicatefound = false;
                foreach (INSTALLED u2 in target)
                    if (u1.name == u2.name)
                    {
                        duplicatefound = true;
                        NewTarget.Add(u2);
                        break;
                    }

                if (!duplicatefound)
                    NewSource.Add(u1);
            }
            return new List<List<INSTALLED>>(){NewSource,NewTarget};
        }
    }
}
