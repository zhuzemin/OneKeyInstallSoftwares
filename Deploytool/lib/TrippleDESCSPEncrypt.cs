using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Deploytool.lib
{
    public class TrippleDESCSPEncrypt
    {
        //12个字符
        private static string customIV = "VBNGCjeHJLK=";
        //32个字符
        private static string customKey = "qJzGVFEGESZDVJeCnFPBNsvnhB7NLQM5";

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string EncryptPassword(string password)
        {
            string encryptPassword = string.Empty;

            SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
            algorithm.Key = Convert.FromBase64String(customKey);
            algorithm.IV = Convert.FromBase64String(customIV);
            algorithm.Mode = CipherMode.ECB;
            algorithm.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = algorithm.CreateEncryptor();

            byte[] data = (new System.Text.ASCIIEncoding()).GetBytes(password);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);

            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            encryptPassword = Convert.ToBase64String(memoryStream.ToArray());

            memoryStream.Close();
            cryptoStream.Close();

            return encryptPassword;
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string DecryptPassword(string password)
        {
            string decryptPassword = string.Empty;

            SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
            algorithm.Key = Convert.FromBase64String(customKey);
            algorithm.IV = Convert.FromBase64String(customIV);
            algorithm.Mode = CipherMode.ECB;
            algorithm.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

            byte[] buffer = Convert.FromBase64String(password);
            MemoryStream memoryStream = new MemoryStream(buffer);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream, System.Text.Encoding.ASCII);
            decryptPassword = reader.ReadToEnd();

            reader.Close();
            cryptoStream.Close();
            memoryStream.Close();

            return decryptPassword;
        }
    }
}