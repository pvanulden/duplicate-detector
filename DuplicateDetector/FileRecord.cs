using System.Collections.Generic;

namespace DuplicateDetector
{
    public class FileRecord
    {
        public List<string> Paths { get; set; } = new List<string>();
        public string MD5 { get; set; }
        public long Length { get; set; }
        public int Copies { get { return Paths.Count; } }
        public int Duplicates { get { return Copies - 1; } }
        public long DuplicateBytes { get { return Duplicates * Length; } }
        public long TotalBytes { get { return Copies * Length; } }
    }
}
