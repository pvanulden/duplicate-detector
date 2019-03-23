using System;
using System.IO;
using System.Security.Cryptography;

namespace DuplicateDetector
{
    public class FileUtility
    {
        public static string GetFileMD5(string file)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        public static long GetFileLength(string file)
        {
            return new FileInfo(file).Length;
        }

        public static string FormatBytes(long bytes)
        {
            long kbytes = bytes / 1024;
            long mbytes = kbytes / 1024;
            long gbytes = mbytes / 1024;
            long tbytes = gbytes / 1024;

            if (kbytes == 0)
            {
                return $"{bytes} bytes";
            }
            else if (mbytes == 0)
            {
                return $"{kbytes} KB";
            }
            else if (gbytes == 0)
            {
                return $"{mbytes} MB";
            }
            else if (tbytes == 0)
            {
                return $"{gbytes} GB";
            }
            else
            {
                return $"{tbytes} TB";
            }
        }
    }
}
