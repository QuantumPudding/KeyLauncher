using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLauncher
{
    public partial class FileBrowser : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        DirectoryInfo currPath;
        DriveInfo[] drives;
        int currDrive;

        List<DirectoryInfo> history;
        int historyIndex = -1;

        public string SelectedPath;

        List<Icon> iconcache;

        public FileBrowser()
        {
            InitializeComponent();

            ImageList il = new ImageList();
            il.ImageSize = new Size(32, 32);

            drives = DriveInfo.GetDrives();

            foreach (DriveInfo d in drives)
                il.Images.Add(d.Name, GetIcons.GetFolderIcon(d.RootDirectory.FullName).ToBitmap());

            lvDrives.LargeImageList = il;

            foreach (DriveInfo d in drives)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Name = d.Name;
                lvi.Text = d.Name;
                lvi.ImageKey = d.Name;

                lvDrives.Items.Add(lvi);
            }

            history = new List<DirectoryInfo>();
            iconcache = new List<System.Drawing.Icon>();

            LoadDirectory(drives[0].RootDirectory);
        }

        private void LoadDirectory(DirectoryInfo root)
        {
            if (root == null)
                return;

            txtPath.Text = root.FullName;

            FileAttributes fa = File.GetAttributes(root.FullName);
            if ((fa & FileAttributes.Directory) != FileAttributes.Directory)
            {
                btnOK.PerformClick();
                return;
            }

            lvFiles.Clear();

            ImageList il = new ImageList();
            il.ImageSize = new Size(16, 16);

            il.Images.Add("folder", GetIcons.GetFolderIcon(root.GetDirectories()[0].FullName).ToBitmap());

            foreach (FileInfo f in root.GetFiles())
            {
                if (f.Extension != ".exe")
                {
                    if (!il.Images.ContainsKey(f.Extension))
                    {
                        il.Images.Add(f.Extension, GetIcons.GetFolderIcon(f.FullName).ToBitmap());
                    }
                }
                else il.Images.Add(f.Name, GetIcons.GetFolderIcon(f.FullName).ToBitmap());

            }

            lvFiles.LargeImageList = il;

            List<ListViewItem> items = new List<ListViewItem>();

            foreach (DirectoryInfo d in root.GetDirectories())
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Name = d.Name;
                lvi.Text = d.Name;
                lvi.Tag = d.FullName;
                lvi.ImageKey = "folder";

                items.Add(lvi);
            }

            foreach (FileInfo f in root.GetFiles())
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Name = f.Name;
                lvi.Text = f.Name;
                lvi.Tag = f.FullName;

                if (f.Extension != ".exe")
                    lvi.ImageKey = f.Extension;
                else lvi.ImageKey = f.Name;

                items.Add(lvi);
            }

            lvFiles.BeginUpdate();
            lvFiles.Items.AddRange(items.ToArray());
            lvFiles.EndUpdate();

            if (!history.Contains(root))
            {
                history.Add(root);
                historyIndex++;
            }
        }

        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void lvDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDrives.SelectedItems.Count > 0)
            {
                LoadDirectory(drives[lvDrives.SelectedIndices[0]].RootDirectory);
                currDrive = lvDrives.SelectedIndices[0];
                txtFilePath.Text = lvDrives.Items[lvDrives.SelectedIndices[0]].Text;
            }
        }

        private void lvFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count > 0)
                LoadDirectory(new DirectoryInfo(lvFiles.Items[lvFiles.SelectedIndices[0]].Tag.ToString()));
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            historyIndex--;

            if (historyIndex < 0)
                historyIndex = 0;

            LoadDirectory(history[historyIndex]);
        }
        
        private void btnNext_Click(object sender, EventArgs e)
        {
            historyIndex++;

            if (historyIndex > history.Count - 1)
                historyIndex = history.Count - 1;

            LoadDirectory(history[historyIndex]);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            LoadDirectory(new DirectoryInfo(txtPath.Text).Parent);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedPath = txtFilePath.Text;
            Close();
        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            txtFilePath.Text = lvFiles.Items[lvFiles.SelectedIndices[0]].Tag.ToString();
        }

        private void lvFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (lvFiles.SelectedItems.Count > 0)
                    LoadDirectory(new DirectoryInfo(lvFiles.Items[lvFiles.SelectedIndices[0]].Tag.ToString()));
            }
        }

        private void FileBrowser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FileBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            GC.Collect();
        }
    }
}
