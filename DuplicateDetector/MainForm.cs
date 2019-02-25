using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace DuplicateDetector
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private FileDatabase _database;
        private DateTime _lastLog;
        private const int _logInterval = 5;

        private void Initialize()
        {
            _database = new FileDatabase();
            _lastLog = DateTime.UtcNow;
            outputTextBox.Text = string.Empty;
        }

        private void EnableControls(bool b)
        {
            directoryTextBox.Enabled = b;
            browseButton.Enabled = b;
            searchButton.Enabled = b;
            searchButton.Visible = b;
            cancelButton.Enabled = !b;
            cancelButton.Visible = !b;
        }

        private void browseButton_Click(object sender, EventArgs e)
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

        private void searchButton_Click(object sender, EventArgs e)
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

            backgroundWorker.RunWorkerAsync(directoryTextBox.Text);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Cancel the asynchronous operation.
            backgroundWorker.CancelAsync();

            // Disable the Cancel button.
            cancelButton.Enabled = false;
        }

        private void DetectDuplicates(string directory, BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    if (!_database.AddRecord(file))
                    {
                        // Duplicate!
                        OutputFile(file, "(duplicate)");
                    }

                    if (Utility.GetElapsedSeconds(_lastLog) > _logInterval)
                    {
                        OutputText($"Scanning ({_database.TotalCount} files)...");
                        _lastLog = DateTime.UtcNow;
                    }
                }

                foreach (string dir in Directory.GetDirectories(directory))
                {
                    DetectDuplicates(dir, worker, e);
                }
            }
            catch (Exception ex)
            {
                OutputText(ex.Message);
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
            if (InvokeRequired)
            {
                Invoke(new Action<string>(OutputText), new object[] { text });
                return;
            }
            outputTextBox.AppendText($"{text}{Environment.NewLine}");
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            DetectDuplicates((string)e.Argument, worker, e);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                OutputText($"Canceled");
            }
            else
            {
                OutputText("-------------------------------------------------------------------");
                OutputText($"{_database.TotalCount} ({Utility.FormatBytes(_database.TotalBytes)}) total files, {_database.DuplicateCount} duplicates ({Utility.FormatBytes(_database.DuplicateBytes)})");
            }

            EnableControls(true);
        }
    }
}
