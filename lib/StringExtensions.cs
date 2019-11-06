using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deploytool.Object;

namespace Deploytool.lib
{
    static class StringExtensions
    {
        //Split string with length
        public static List<string> GetChunkss(string value, int chunkSize)
        {
            List<string> triplets = new List<string>();
            for (int i = 0; i < value.Length; i += chunkSize)
                if (i + chunkSize > value.Length)
                    triplets.Add(value.Substring(i));
                else
                    triplets.Add(value.Substring(i, chunkSize));

            return triplets;
        }

        
    }
}
