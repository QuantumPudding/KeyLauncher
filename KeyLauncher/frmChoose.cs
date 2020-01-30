using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public partial class frmChoose : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        Form1 parent;
        StreamWriter sw;

        public frmChoose(Form1 parent)
        {
            this.parent = parent;
            InitializeComponent();

            //sw = new StreamWriter("Config", true);
        }

        public frmChoose(Form1 parent, string key, string tag)
        {
            this.parent = parent;
            InitializeComponent();

            txtKey.Text = key;
            txtAction.Text = tag;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            StringCollection sc = new StringCollection();
            try
            {
                sw = new StreamWriter("Config");
                if (txtKey.Text != "")
                {
                    foreach (Control c in parent.Controls[0].Controls)
                    {
                        if (c.Name.StartsWith("button"))
                        {
                            Button b = (Button)c;

                            if (c.Name.EndsWith(txtKey.Text))
                            {
                                b.Tag = txtAction.Text;

                                if (b.Tag.ToString() == "")
                                {
                                    b.Tag = null;
                                    b.BackgroundImage = null;
                                }
                            }

                            if (b.Tag != null)
                            {
                                try
                                {
                                    b.BackgroundImage = Icon.ExtractAssociatedIcon(b.Tag.ToString()).ToBitmap();
                                }
                                catch
                                {
                                    try
                                    {
                                        b.BackgroundImage = GetIcons.GetFolderIcon(b.Tag.ToString(), GetIcons.IconSize.Large, GetIcons.FolderType.Open).ToBitmap();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }

                                sc.Add(b.Text + "|" + b.Tag);
                                //sw.WriteLine(b.Text + "|" + b.Tag);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //sw.Flush();
                //sw.Close();

                Properties.Settings.Default["Keys"] = sc;
                Properties.Settings.Default.Save();

                Close();
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            FileBrowser dlg = new FileBrowser();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtAction.Text = dlg.SelectedPath;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void frmChoose_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                btnApply.PerformClick();
            }
        }
    }
}
