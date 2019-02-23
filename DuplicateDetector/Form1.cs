using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuplicateDetector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Dictionary<string, string> _md5Map;
        private List<string> _duplicates;
        private DateTime _lastLog;

        private void Initialize()
        {
            _md5Map = new Dictionary<string, string>();
            _duplicates = new List<string>();
            _lastLog = DateTime.UtcNow;
        }

        private void EnableControls(bool b)
        {
            directoryTextBox.Enabled = b;
            button1.Enabled = b;
            button2.Enabled = b;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false
            };

            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            directoryTextBox.Text = dlg.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(directoryTextBox.Text))
            {
                return;
            }

            if (!Directory.Exists(directoryTextBox.Text))
            {
                return;
            }

            Initialize();
            EnableControls(false);
            DetectDuplicates(directoryTextBox.Text);
            EnableControls(true);

            OutputText("-------------------------------------------------------------------");
            OutputText($"Found {_duplicates.Count} duplicates out of {_md5Map.Count + _duplicates.Count} total files.");
        }

        private string GetMD5(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        private void DetectDuplicates(string directory)
        {
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    string md5 = GetMD5(file);

                    if (_md5Map.TryGetValue(md5, out string orig))
                    {
                        _duplicates.Add(file);
                        OutputFile(orig, "(original)");
                        OutputFile(file, "(duplicate)");
                    }
                    else
                    {
                        _md5Map.Add(md5, file);
                    }

                    DateTime now = DateTime.UtcNow;
                    if ((now - _lastLog).TotalSeconds > 5)
                    {
                        OutputText($"Scanning ({_md5Map.Count + _duplicates.Count} files)...");
                        _lastLog = now;
                    }
                }

                foreach (string dir in Directory.GetDirectories(directory))
                {
                    DetectDuplicates(dir);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void OutputFile(string file, string suffix)
        {
            string outputText = $"{file} {suffix}";
            outputText = outputText.Replace(directoryTextBox.Text + Path.DirectorySeparatorChar, "");
            OutputText(outputText);
        }

        private void OutputText(string text)
        {
            textBox1.AppendText($"{text}{Environment.NewLine}");
        }
    }
}
