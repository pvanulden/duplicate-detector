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
            double kbytes = bytes / 1024f;
            double mbytes = kbytes / 1024f;
            double gbytes = mbytes / 1024f;
            double tbytes = gbytes / 1024f;

            if (kbytes < 1)
            {
                return $"{bytes} bytes";
            }
            else if (mbytes < 1)
            {
                return $"{kbytes:0.##} KB";
            }
            else if (gbytes < 1)
            {
                return $"{mbytes:0.##} MB";
            }
            else if (tbytes < 1)
            {
                return $"{gbytes:0.##} GB";
            }
            else
            {
                return $"{tbytes:0.##} TB";
            }
        }
    }
}
