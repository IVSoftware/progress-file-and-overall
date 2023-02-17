using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace progress_file_and_overall
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            tsmiBrowseFolder.Click += onClickBrowseFolder;
        }

        private readonly FolderBrowserDialog _folderBrowser = new FolderBrowserDialog
        {
            RootFolder= Environment.SpecialFolder.ApplicationData,
        };
        private void onClickBrowseFolder(object sender, EventArgs e)
        {
            if(_folderBrowser.ShowDialog() == DialogResult.OK) 
            {
                _ = loadFilesAsync(_folderBrowser.SelectedPath);
            }
        }

        const int ONE_MB = 0x100000;
        private async Task loadFilesAsync(string selectedPath)
        {
            string[] files = Directory.GetFiles(selectedPath, "*.*", SearchOption.AllDirectories);

            for (int count = 0; count < files.Length; count++)
            {
                progressBarOverall.Value = (int)((count/(float)files.Length) * 100);
                string path = files[count];

                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    int offset = 0;
                    long length = fileStream.Length;
                    byte[] buffer = new byte[length];
                    int readLen = ONE_MB;
                    while (offset != length)
                    {
                        if (offset + readLen > length)
                        {
                            readLen = (int)length - offset;
                        }
                        else
                        {   /* G T K */
                        }
                        offset += await fileStream.ReadAsync(buffer, offset, readLen);
                        progressBarSingle.Value = (int)((offset / (float)length) * 100);
                    }
                }
            }
            progressBarOverall.Visible = false;
        }
    }
}
