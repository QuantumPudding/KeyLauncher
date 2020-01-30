using Etier.IconHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLauncher
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        static void MinimizeFootprint()
        {
            EmptyWorkingSet(Process.GetCurrentProcess().Handle);
        }

        Hotkey hk;

        public Form1()
        {
            InitializeComponent();

            List<string> keys = new List<string>();
            try
            {
                //StreamReader sr = new StreamReader("Config");
                //while (sr.Peek() != -1)
                //    keys.Add(sr.ReadLine());
                //sr.Close();

                keys = ((StringCollection)Properties.Settings.Default["Keys"]).Cast<string>().ToList();
            }
            catch { }

            foreach (string s in keys)
            {
                string[] data = s.Split('|');
                foreach (Control c in Controls[0].Controls)
                {
                    if (c.Name.StartsWith("button"))
                    {
                        if (c.Name.EndsWith(data[0]))
                        {
                            Button b = (Button)c;

                            try
                            {
                                b.BackgroundImage = Icon.ExtractAssociatedIcon(data[1]).ToBitmap();
                            }
                            catch
                            {
                                try
                                {
                                    b.BackgroundImage = GetIcons.GetFolderIcon(data[1], GetIcons.IconSize.Large, GetIcons.FolderType.Open).ToBitmap();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, data[1]);
                                }
                            }
                            b.Tag = data[1];
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            hk = new Hotkey(Constants.WIN, Keys.G, this);
            hk.Register();

            MinimizeFootprint();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
                Visible = !Visible;

            base.WndProc(ref m);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            hk.Unregiser();
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Button c = (Button)sender;
            c.BackColor = Color.FromArgb(128, 128, 128);
            Refresh();

            c.PerformClick();

            System.Threading.Thread.Sleep(50);
            c.BackColor = Color.FromArgb(48, 48, 48);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'a':
                    ButtonClick(buttonA, null);
                    break;
                case 'b':
                    ButtonClick(buttonB, null);
                    break;
                case 'c':
                    ButtonClick(buttonC, null);
                    break;
                case 'd':
                    ButtonClick(buttonD, null);
                    break;
                case 'e':
                    ButtonClick(buttonE, null);
                    break;
                case 'f':
                    ButtonClick(buttonF, null);
                    break;
                case 'g':
                    ButtonClick(buttonG, null);
                    break;
                case 'h':
                    ButtonClick(buttonH, null);
                    break;
                case 'i':
                    ButtonClick(buttonI, null);
                    break;
                case 'j':
                    ButtonClick(buttonJ, null);
                    break;
                case 'k':
                    ButtonClick(buttonK, null);
                    break;
                case 'l':
                    ButtonClick(buttonL, null);
                    break;
                case 'm':
                    ButtonClick(buttonM, null);
                    break;
                case 'n':
                    ButtonClick(buttonN, null);
                    break;
                case 'o':
                    ButtonClick(buttonO, null);
                    break;
                case 'p':
                    ButtonClick(buttonP, null);
                    break;
                case 'q':
                    ButtonClick(buttonQ, null);
                    break;
                case 'r':
                    ButtonClick(buttonR, null);
                    break;
                case 's':
                    ButtonClick(buttonS, null);
                    break;
                case 't':
                    ButtonClick(buttonT, null);
                    break;
                case 'u':
                    ButtonClick(buttonU, null);
                    break;
                case 'v':
                    ButtonClick(buttonV, null);
                    break;
                case 'w':
                    ButtonClick(buttonW, null);
                    break;
                case 'x':
                    ButtonClick(buttonX, null);
                    break;
                case 'y':
                    ButtonClick(buttonY, null);
                    break;
                case 'z':
                    ButtonClick(buttonZ, null);
                    break;
                case '0':
                    ButtonClick(button0, null);
                    break;
                case '1':
                    ButtonClick(button1, null);
                    break;
                case '2':
                    ButtonClick(button2, null);
                    break;
                case '3':
                    ButtonClick(button3, null);
                    break;
                case '4':
                    ButtonClick(button4, null);
                    break;
                case '5':
                    ButtonClick(button5, null);
                    break;
                case '6':
                    ButtonClick(button6, null);
                    break;
                case '7':
                    ButtonClick(button7, null);
                    break;
                case '8':
                    ButtonClick(button8, null);
                    break;
                case '9':
                    ButtonClick(button9, null);
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
                Application.Exit();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            panel1.Focus();
        }

        private void OnButtonResize(object sender, EventArgs e)
        {
            
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void ButtonMouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Tag != null)
            {
                string s = b.Tag.ToString();
                try
                {
                    System.Diagnostics.Process.Start(s);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            panel1.Focus();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void OnButtonMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button b = (Button)sender;

                frmChoose frm = new frmChoose(this, b.Text, (b.Tag != null) ? b.Tag.ToString() : "");
                frm.Show();
            }
        }

        private void ButtonMouseHover(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Tag == null)
                return;

            string s = b.Tag.ToString();
            int i = s.LastIndexOf("\\") + 1;

            toolTip.Show(s.Substring(i), this, ((Button)sender).Location, 300);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Visible = !Visible;
            else if (e.Button == MouseButtons.Right)
                Application.Exit();
        }
    }
}
