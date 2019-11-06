using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Shell32;
using System.Security.Permissions;

namespace Deploytool.lib
{
    public class FILEUTILS
    {
        public string path { get; set; }
        public string target { get; set; }
        public FILEUTILS()
        {
            path = Directory.GetCurrentDirectory();
            target=@"D:\部署工具\";
            string drive = Path.GetPathRoot(target);   // e.g. K:\

            if (!Directory.Exists(drive)) {
                target = @"C:\部署工具\";
            }
        }
        //list your excluded dirs
        private List<string> _excludedDirectories = new List<string>() { "softs" };

        //method to check
        static bool isExcluded(List<string> exludedDirList, string target)
        {
            return exludedDirList.Any(d => new DirectoryInfo(target).Name.Equals(d));
        }

        //then use this
        public void copyFolder(string sourceDirectory, string targetDirectory)
        {
            var filteredDirs = Directory.GetDirectories(path).Where(d => !isExcluded(_excludedDirectories, d));
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(sourceDirectory, targetDirectory,true);
        }
      
        public string[] searchFile(string path , string pattern,string second_pattern=null,Boolean SearchSubDir=true)
        {
            if (second_pattern == null)
            {
                second_pattern = pattern;
            }
            String[] files;
            if(SearchSubDir)
                files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
            else
                files = Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);
            List<string> TempList = new List<string>();

            foreach (string u1 in files)
            {
                if (u1.Contains(second_pattern) || second_pattern == pattern)
                    {
                        TempList.Add(u1);
                    }

            }
            return TempList.ToArray(); 
            //return files;
        }
        public string GetShortcutTargetFile(string shortcutFilename)
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return string.Empty;
        }

        public void append(string content, string path)
        {
        // This text is added only once to the file.
        if (!File.Exists(path)) 
        {
            // Create a file to write to.

            using (StreamWriter sw = File.CreateText(path)) 
            {
                sw.WriteLine(content);
            }	
        }

        // This text is always added, making the file longer over time
        // if it is not deleted.
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(content);
        }
        }

        //find all file and folder in path
        public IEnumerable<string> Traverse(string rootDirectory)
        {
            IEnumerable<string> files = Enumerable.Empty<string>();
            IEnumerable<string> directories = Enumerable.Empty<string>();
            try
            {
                // The test for UnauthorizedAccessException.
                var permission = new FileIOPermission(FileIOPermissionAccess.PathDiscovery, rootDirectory);
                permission.Demand();

                files = Directory.GetFiles(rootDirectory);
                directories = Directory.GetDirectories(rootDirectory);
            }
            catch
            {
                // Ignore folder (access denied).
                rootDirectory = null;
            }

            if (rootDirectory != null)
                yield return rootDirectory;

            foreach (var file in files)
            {
                yield return file;
            }
            foreach (var directory in directories)
            {
                yield return directory;
            }

            // Recursive call for SelectMany.
            /*var subdirectoryItems = directories.SelectMany(Traverse);
            foreach (var result in subdirectoryItems)
            {
                yield return result;
            }*/
        }
    }
}
