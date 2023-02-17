One of many ways is with a for-next loop on the count of the `Directory.GetAllFiles()` result and then use the `FileStream.ReadAsync()` method to read (for example) 1 MB chunks without blocking the UI. The ProgressBar updates will be on the UI thread in this way, as they should be.

[![screenshot][1]][1]

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
                int readLength = ONE_MB;
                while (offset != length)
                {
                    if (offset + readLength > length)
                    {
                        readLength = (int)length - offset;
                    }
                    offset += await fileStream.ReadAsync(buffer, offset, readLength);
                    progressBarSingle.Value = (int)((offset / (float)length) * 100);
                }
            }
        }
        tableLayoutPanelStatus.Visible = false;
    }

***
**Testing**

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
        .
        .
        .        
    }

  [1]: https://i.stack.imgur.com/MhUBP.png