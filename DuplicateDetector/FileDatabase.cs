using System.Collections.Generic;

namespace DuplicateDetector
{
    public class FileDatabase
    {
        private readonly Dictionary<string, FileRecord> _records = new Dictionary<string, FileRecord>();

        public int TotalCount { get; set; }
        public long TotalBytes { get; set; }
        public int DuplicateCount { get; set; }
        public long DuplicateBytes { get; set; }

        /// <summary>
        /// Returns true if the file is not a duplicate
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool AddRecord(string file)
        {
            FileRecord record = new FileRecord(file);

            TotalCount++;
            TotalBytes += record.Length;

            if (_records.TryGetValue(record.MD5, out FileRecord existing))
            {
                existing.Add(file);
                DuplicateCount++;
                DuplicateBytes += record.Length;
                return false;
            }
            else
            {
                _records.Add(record.MD5, record);
                return true;
            }
        }
    }
}
