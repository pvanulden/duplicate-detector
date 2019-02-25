using System.Collections.Generic;

namespace DuplicateDetector
{
    public class FileRecord
    {
        public FileRecord(string path)
        {
            Add(path);
            Length = Utility.GetFileLength(path);
            MD5 = Utility.GetFileMD5(path);
        }
        public void Add(string path)
        {
            if (null == Paths)
                Paths = new List<string>();

            Paths.Add(path);
        }
        public List<string> Paths { get; set; }
        public string MD5 { get; set; }
        public long Length { get; set; }
        public int Copies { get { return Paths.Count; } }
    }
}
