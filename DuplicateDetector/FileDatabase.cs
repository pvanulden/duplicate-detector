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
            FileRecord record = FileRecordExtension.Create(file);

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

        private void ResetStats()
        {
            TotalCount = 0;
            TotalBytes = 0;
            DuplicateCount = 0;
            DuplicateBytes = 0;
        }

        public void RefreshStats()
        {
            ResetStats();

            TotalCount = _records.Count;

            foreach (KeyValuePair<string, FileRecord> entry in _records)
            {
                FileRecord record = entry.Value;
                TotalBytes += record.TotalBytes;
                DuplicateCount += record.Duplicates;
                DuplicateBytes += record.DuplicateBytes;
            }
        }
    }
}
