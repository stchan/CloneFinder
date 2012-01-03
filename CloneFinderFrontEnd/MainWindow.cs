using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CloneFinderFrontEnd
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            WireEvents();
            Utility.PositionStartupForm(this);
            InitializeDataGridView(this.dataGridViewResults);
        }

        private const string exitDialogCaption = "Exit";
        private const string exitDialogMessage = "Are you sure you want to exit?";
        private const string propertySuffix = "GridColumnWidth_";

        private bool saveSizesAndPositions = true;

        enum gridColumnSavedWidth
        {
            FilePath = 0,
            Name,
            Length,
            LastModified,
            FileHash,
            EndLoop
        }

        private void InitializeDataGridView(DataGridView gridView)
        {

            gridView.Columns.Clear();            
            gridView.AutoGenerateColumns = false;
            gridView.AllowUserToResizeColumns = true;
            Font gridFont = new Font(gridView.Parent.Font.FontFamily, (float)(gridView.Parent.Font.SizeInPoints * .75D)); 
            gridView.Font = gridFont;
            for (int loop = 0; loop < (int)gridColumnSavedWidth.EndLoop; loop++)
            {
                DataGridViewTextBoxColumn newColumn = new DataGridViewTextBoxColumn();
                switch (loop)
                {
                    case ((int)gridColumnSavedWidth.FilePath):
                        newColumn.DataPropertyName = "FilePath";
                        newColumn.HeaderText = "Path";
                        break;
                    case ((int)gridColumnSavedWidth.Name):
                        newColumn.DataPropertyName = "Name";
                        newColumn.HeaderText = "Filename";
                        break;
                    case ((int)gridColumnSavedWidth.Length):
                        newColumn.DataPropertyName = "Length";
                        newColumn.HeaderText = "Length";
                        newColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    case ((int)gridColumnSavedWidth.LastModified):
                        newColumn.DataPropertyName = "LastModified";
                        newColumn.HeaderText = "Last Modified";
                        break;
                    case ((int)gridColumnSavedWidth.FileHash):
                        newColumn.DataPropertyName = "FileHash";
                        newColumn.HeaderText = "Hash Value";
                        break;
                    default:
                        newColumn = null;
                        break;
                }
                if (newColumn != null)
                {
                    newColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    newColumn.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
                    newColumn.Resizable = DataGridViewTriState.True;
                    newColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                    gridView.Columns.Add(newColumn);
                    // Can't change the size until it's added to the grid because
                    // we can't compute the preferred width until it's in the grid
                    if ((int)Properties.Settings.Default[propertySuffix + newColumn.DataPropertyName] > 0) newColumn.Width = (int)Properties.Settings.Default[propertySuffix + newColumn.DataPropertyName];
                    else newColumn.Width = newColumn.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true);
                }
            }


        }

        private static void SaveGridColumnWidths(DataGridView gridView)
        {
            foreach (DataGridViewTextBoxColumn gridColumn in gridView.Columns)
            {
                if (Properties.Settings.Default[propertySuffix + gridColumn.DataPropertyName] != null)
                {
                    Properties.Settings.Default[propertySuffix + gridColumn.DataPropertyName] = gridColumn.Width;
                }
            }
            Properties.Settings.Default.Save();
        }

        #region Event Handlers

        private void WireEvents()
        {
            this.searchToolStripMenuItem.Click += new EventHandler(searchToolStripMenuItem_Click);
            this.exitToolStripMenuItem.Click += new EventHandler(exitToolStripMenuItem_Click);
            this.aboutToolStripMenuItem.Click += new EventHandler(aboutToolStripMenuItem_Click);
            this.toolsResetDefaultSizestoolStripMenuItem.Click += new EventHandler(toolsResetDefaultSizestoolStripMenuItem_Click);
            this.buttonBrowse.Click += new EventHandler(buttonBrowse_Click);
            this.buttonSearch.Click += new EventHandler(buttonSearch_Click);
            this.buttonCancel.Click += new EventHandler(buttonCancel_Click);
            this.backgroundWorkerSearch.DoWork += new DoWorkEventHandler(backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerSearch_ProgressChanged);
            this.backgroundWorkerSearch.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerSearch_RunWorkerCompleted);
            this.timerProgress.Tick += new EventHandler(timerProgress_Tick);
            this.FormClosing +=new FormClosingEventHandler(MainWindow_FormClosing);            
        }

        void toolsResetDefaultSizestoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetSizeDefaults();
            Application.Restart();
        }

        private void ResetSizeDefaults()
        {
            foreach (SettingsProperty savedSize in Properties.Settings.Default.Properties)
            {
                if (savedSize.IsReadOnly != true)
                {
                    switch (savedSize.Name)
                    {
                        case "LastXPosition":
                        case "LastYPosition":
                            //savedSize.DefaultValue = -1;
                            Properties.Settings.Default[savedSize.Name] = -1;
                            break;
                        default:
                            Properties.Settings.Default[savedSize.Name] = 0;
                            break;
                    }
                }
            }
            Properties.Settings.Default.Save();
            this.saveSizesAndPositions = false;
        }

        void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox aboutDialog = new AboutBox())
            {
                aboutDialog.ShowDialog();
            }
        }

        private void MainWindow_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall && e.CloseReason != CloseReason.WindowsShutDown &&
                (MessageBox.Show(exitDialogMessage, exitDialogCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.No))
            {
                e.Cancel = true;
            }
            else
            {
                if (this.saveSizesAndPositions == true)
                {
                    Utility.SaveMainFormPosition(this);
                    SaveGridColumnWidths(this.dataGridViewResults);
                }
            }
        }

        void timerProgress_Tick(object sender, EventArgs e)
        {
            if (this.progressBarSearch.Value >= this.progressBarSearch.Maximum) this.progressBarSearch.Value = this.progressBarSearch.Minimum;
            this.progressBarSearch.PerformStep();
        }

        void buttonCancel_Click(object sender, EventArgs e)
        {
            // Yeah I know this is a really nasty way to
            // stop a search, but there's no quick and clean way
            // stop any backgroundworker code that
            // doesn't listen for a cancellation
            try
            {
                Utility.SaveMainFormPosition(this);
            }
            finally
            {
                Application.Restart();
            }
        }

        void backgroundWorkerSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this.dataGridViewResults.DataSource = null;
                Collection<CloneFinderCore.ProcessedFileInfo> duplicates = e.Result as Collection<CloneFinderCore.ProcessedFileInfo>;
                if (duplicates != null && duplicates.Count > 0)
                {
                    this.dataGridViewResults.DataSource = duplicates;
                    this.labelStatus.Text = duplicates.Count.ToString() + " results. Duplicate files will have the same hash value.";
                }
                else
                {
                    MessageBox.Show("No duplicates were found in this folder.", "No duplicates", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.labelStatus.Text = "No duplicate files found.";
                }
            }
            else
            {
                this.labelStatus.Text = "Search cancelled.";
            }
            this.timerProgress.Enabled = false;
            this.buttonCancel.Visible = false;
            EnableUserControls();
        }

        private void backgroundWorkerSearch_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            String progressMessage = e.UserState as String;
            if (progressMessage != null)
            {
                this.labelStatus.Text = progressMessage;
            }
        }

        void backgroundWorkerSearch_DoWork(object sender, DoWorkEventArgs e)
        {
           
            CloneFinderCore.DirectoryWalker directoryWalk = new CloneFinderCore.DirectoryWalker(this.textBoxSearchPath.Text);
            directoryWalk.FileProcessed += new EventHandler<CloneFinderCore.DirectoryWalkEventArgs>(directoryWalk_FileProcessed);
            directoryWalk.FileAccessed += new EventHandler<CloneFinderCore.DirectoryWalkEventArgs>(directoryWalk_FileAccessed);
            directoryWalk.DirectoryWalkComplete += new EventHandler<CloneFinderCore.DirectoryWalkEventArgs>(directoryWalk_DirectoryWalkComplete);
            Collection<CloneFinderCore.ProcessedFileInfo> duplicates = directoryWalk.WalkDirectory();
            e.Result = duplicates;
        }

        void directoryWalk_DirectoryWalkComplete(object sender, CloneFinderCore.DirectoryWalkEventArgs e)
        {
            this.backgroundWorkerSearch.ReportProgress(0, e.Message);
        }

        void directoryWalk_FileAccessed(object sender, CloneFinderCore.DirectoryWalkEventArgs e)
        {
            this.backgroundWorkerSearch.ReportProgress(0, e.Message + " " + e.FullPath + e.Name);
        }

        void directoryWalk_FileProcessed(object sender, CloneFinderCore.DirectoryWalkEventArgs e)
        {
            this.backgroundWorkerSearch.ReportProgress(0, e.Message + " " + e.FullPath + e.Name);
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }




        void buttonSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SearchBackGround();
            }
            finally
            {
            }
        }

        void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.textBoxSearchPath.Text) &&
                Directory.Exists(this.textBoxSearchPath.Text))
            {
                this.folderBrowserDialogSearch.SelectedPath = this.textBoxSearchPath.Text;
            }
            else
            {
                this.folderBrowserDialogSearch.SelectedPath = String.Empty;
            }
            this.folderBrowserDialogSearch.ShowDialog();
            if (!String.IsNullOrEmpty(this.folderBrowserDialogSearch.SelectedPath))
            {
                this.textBoxSearchPath.Text = this.folderBrowserDialogSearch.SelectedPath;
            }
            else
            {
                this.textBoxSearchPath.Text = String.Empty;
            }
        }

        void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.searchToolStripMenuItem.Enabled = false;
            try
            {
                Search();
            }
            finally
            {
                this.searchToolStripMenuItem.Enabled = true;
            }
        }

        private bool IsSearchPathValid(string searchPath)
        {
            bool pathExists = Directory.Exists(searchPath);

            return pathExists;
        }

        private void SearchBackGround()
        {
            // Make sure we're searching on a valid folder
            if (Directory.Exists(this.textBoxSearchPath.Text))
            {
                this.Cursor = Cursors.AppStarting;
                try
                {
                    this.labelStatus.Text = String.Empty;
                    this.dataGridViewResults.DataSource = null;
                    DisableUserControls();
                    this.buttonCancel.Visible = true;
                    this.backgroundWorkerSearch.RunWorkerAsync();
                    this.progressBarSearch.Visible = true;
                    this.timerProgress.Enabled = false;
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else
            {
                MessageBox.Show("A valid folder to search has not been entered/selected.", "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (!String.IsNullOrEmpty(this.folderBrowserDialogSearch.SelectedPath) && Directory.Exists(this.folderBrowserDialogSearch.SelectedPath))
                {
                    this.textBoxSearchPath.Text = this.folderBrowserDialogSearch.SelectedPath;
                }
                this.textBoxSearchPath.Focus();
            }

        }

        private void Search()
        {
            // Make sure we're searching on a valid folder
            if (Directory.Exists(this.textBoxSearchPath.Text))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    this.labelStatus.Text = String.Empty;
                    this.dataGridViewResults.DataSource = null;
                    CloneFinderCore.DirectoryWalker directoryWalk = new CloneFinderCore.DirectoryWalker(this.textBoxSearchPath.Text);
                    Collection<CloneFinderCore.ProcessedFileInfo> duplicates = directoryWalk.WalkDirectory();
                    if (duplicates != null && duplicates.Count > 0)
                    {
                        this.dataGridViewResults.DataSource = duplicates;
                        this.labelStatus.Text = duplicates.Count.ToString() + " duplicate files found.";
                    }
                    else
                    {
                        MessageBox.Show("No duplicates were found in this folder.", "No duplicates", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.labelStatus.Text = "No duplicate files found.";
                    }
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else
            {
                MessageBox.Show("A valid folder to search has not been entered/selected.", "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (!String.IsNullOrEmpty(this.folderBrowserDialogSearch.SelectedPath) && Directory.Exists(this.folderBrowserDialogSearch.SelectedPath))
                {
                    this.textBoxSearchPath.Text = this.folderBrowserDialogSearch.SelectedPath;
                }
                this.textBoxSearchPath.Focus();
            }
        }

        private void DisableUserControls()
        {
            this.buttonSearch.Enabled = false;
            this.buttonBrowse.Enabled = false;
            this.textBoxSearchPath.Enabled = false;
            this.searchToolStripMenuItem.Enabled = false;
            this.editToolStripMenuItem.Enabled = false;
            this.toolsResetDefaultSizestoolStripMenuItem.Enabled = false;
            this.buttonCancel.Enabled = true;
        }

        private void EnableUserControls()
        {
            this.progressBarSearch.Visible = false;
            this.buttonSearch.Enabled = true;
            this.buttonBrowse.Enabled = true;
            this.textBoxSearchPath.Enabled = true;
            this.searchToolStripMenuItem.Enabled = true;
            this.editToolStripMenuItem.Enabled = true;
            this.toolsResetDefaultSizestoolStripMenuItem.Enabled = true;
            this.buttonCancel.Enabled = false;
            
        }

        #endregion

    }
}
