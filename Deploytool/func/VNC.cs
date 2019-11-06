using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deploytool.lib;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace Deploytool.func
{
    class VNC
    {
          FILEUTILS fileutils;
        public string version{ get; set; }
        public string licenseKey { get; set; }
        public string password { get; set; }
        public string SoftName { get; set; }

        public VNC(FILEUTILS fileutils)
        {
            this.fileutils = fileutils;
            //search "Softs" floder for soft path, if exist multiple version, select the newer one
            SoftName = "*vnc*.exe";
            string[] files = fileutils.searchFile(fileutils.path, SoftName);
            version = files.OrderByDescending(path => File.GetLastWriteTime(path)).FirstOrDefault();
            SoftName = version.Split('\\').Last();
        }
       public  void install()
        {
            //licenseKey = "BR43B-6H233-X43PN-Z6RS7-M5ZTA";
            //password = "baidu.com";
            password = EncryptVNC(password);
           //only RealVNC "6" or newer version can applicable "Silent install"(/qn")
            string batch = "start \"\" \"" +  version + "\" /qn REBOOT=ReallySuppress LICENSEKEY=" + licenseKey + " ENABLEAUTOUPDATECHECKS=0 ENABLEANALYTICS=0";
            //Console.WriteLine(batch); 
           cmd.ExecuteCommand(batch);
            string realvnc = @"HKEY_LOCAL_MACHINE\SOFTWARE\RealVNC\vncserver";
            Registry.SetValue(realvnc, "Authentication", "VncAuth");
            Registry.SetValue(realvnc, "Password", password);

        }
       public static string EncryptVNC(string password)
       {
           string whollypass=null;
           List<string> parts=StringExtensions.GetChunkss(password,8);
           foreach (string part in parts)
           {
               /*if (password.Length > 8)
               {
                   password = password.Substring(0, 8);
               }*/
               password = part;
               if (password.Length < 8)
               {
                   password = password.PadRight(8, '\0');
               }
               byte[] key = { 23, 82, 107, 6, 35, 78, 88, 7 };
               byte[] passArr = new ASCIIEncoding().GetBytes(password);
               byte[] response = new byte[passArr.Length];
               char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

               // reverse the byte order
               byte[] newkey = new byte[8];
               for (int i = 0; i < 8; i++)
               {
                   // revert desKey[i]:
                   newkey[i] = (byte)(
                       ((key[i] & 0x01) << 7) |
                       ((key[i] & 0x02) << 5) |
                       ((key[i] & 0x04) << 3) |
                       ((key[i] & 0x08) << 1) |
                       ((key[i] & 0x10) >> 1) |
                       ((key[i] & 0x20) >> 3) |
                       ((key[i] & 0x40) >> 5) |
                       ((key[i] & 0x80) >> 7)
                       );
               }
               key = newkey;
               // reverse the byte order

               DES des = new DESCryptoServiceProvider();
               des.Padding = PaddingMode.None;
               des.Mode = CipherMode.ECB;

               ICryptoTransform enc = des.CreateEncryptor(key, null);
               enc.TransformBlock(passArr, 0, passArr.Length, response, 0);

               string hexString = String.Empty;
               for (int i = 0; i < response.Length; i++)
               {
                   hexString += chars[response[i] >> 4];
                   hexString += chars[response[i] & 0xf];
               }
               whollypass+=hexString.Trim().ToLower();
           }
           return whollypass;
       }
       public static string DecryptVNC(string password)
       {
           if (password.Length < 16)
           {
               return string.Empty;
           }

           byte[] key = { 23, 82, 107, 6, 35, 78, 88, 7 };
           byte[] passArr = ToByteArray(password);
           byte[] response = new byte[passArr.Length];

           // reverse the byte order
           byte[] newkey = new byte[8];
           for (int i = 0; i < 8; i++)
           {
               // revert key[i]:
               newkey[i] = (byte)(
                   ((key[i] & 0x01) << 7) |
                   ((key[i] & 0x02) << 5) |
                   ((key[i] & 0x04) << 3) |
                   ((key[i] & 0x08) << 1) |
                   ((key[i] & 0x10) >> 1) |
                   ((key[i] & 0x20) >> 3) |
                   ((key[i] & 0x40) >> 5) |
                   ((key[i] & 0x80) >> 7)
                   );
           }
           key = newkey;
           // reverse the byte order

           DES des = new DESCryptoServiceProvider();
           des.Padding = PaddingMode.None;
           des.Mode = CipherMode.ECB;

           ICryptoTransform dec = des.CreateDecryptor(key, null);
           dec.TransformBlock(passArr, 0, passArr.Length, response, 0);

           return System.Text.ASCIIEncoding.ASCII.GetString(response);
       }
       public static byte[] ToByteArray(String HexString)
       {
           int NumberChars = HexString.Length;
           byte[] bytes = new byte[NumberChars / 2];

           for (int i = 0; i < NumberChars; i += 2)
           {
               bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
           }

           return bytes;
       }
    }
}
