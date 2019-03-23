using System;

namespace DuplicateDetector
{
    public static class FileRecordExtension
    {
        public static FileRecord Create(string path)
        {
            FileRecord fileRecord = new FileRecord
            {
                Length = FileUtility.GetFileLength(path),
                MD5 = FileUtility.GetFileMD5(path)
            };

            fileRecord.Add(path);

            return fileRecord;
        }

        public static void Add(this FileRecord fileRecord, string path)
        {
            if (fileRecord.Paths.Contains(path))
            {
                throw new ArgumentException($"'{path}' exists");
            }

            fileRecord.Paths.Add(path);
        }
    }
}
