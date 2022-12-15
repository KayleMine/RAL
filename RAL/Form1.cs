    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.IO;
    using Ookii.Dialogs.WinForms;
    using System.Security;
    using System.Net;
using System.Threading;
using SimpleImpersonation;
using System.Text.RegularExpressions;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Authentication;

namespace RAL
    {
    public partial class Form1 : Form
    {
        private int counter;
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        public  string NameLog;
        public  string Pass;
        private void InitializeTimer()
        {
            counter = 0;
            t.Interval = 750;
            t.Enabled = true;

            t.Tick += new EventHandler(timer1_Tick);
        }
        public Form1()
        {
            InitializeComponent();
            InitializeTimer();
            textBox5.ReadOnly = true;
            if (Properties.Settings.Default.third_party_ware_exe != "")
            { textBox5.Text = Properties.Settings.Default.third_party_ware_exe; button12.Enabled = false; }
            if (Properties.Settings.Default.timer != "") { textBox6.Text = Properties.Settings.Default.timer; }
            if (
                    Properties.Settings.Default.user != "" && Properties.Settings.Default.password != ""
                )
                try
                {
                    textBox1.Text = Properties.Settings.Default.user;
                    textBox2.Text = Properties.Settings.Default.password;
                    pictureBox1.BackgroundImage = Properties.Resources.ok48;
                    textBox1.ReadOnly = true;
                    textBox2.ReadOnly = true;
                    button1.Enabled = false;
                }
                catch { }
            else
                try
                {
                    pictureBox1.BackgroundImage = Properties.Resources.no48;
                    textBox1.ReadOnly = false;
                    textBox2.ReadOnly = false;
                    button1.Enabled = true;
                }
                catch { }
                ;
        }
        private const int CS_DropShadow = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = CS_DropShadow;
                return cp;
            }
        }

        private void button1_Click(object sender, EventArgs e) //create
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            try
                {
                    NameLog = textBox1.Text;
                    Pass = textBox2.Text;

                    DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                    DirectoryEntry NewUser = AD.Children.Add(NameLog, "user");
                    NewUser.Invoke("SetPassword", new object[] { Pass });
                    NewUser.Invoke("Put", new object[] { "Description", "" });
                    NewUser.CommitChanges();
                    DirectoryEntry grp;
                    
                    grp = AD.Children.Find("Users", "group");
                    if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
                   

                    Properties.Settings.Default.user = textBox1.Text;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.password = textBox2.Text;
                    Properties.Settings.Default.Save();
                    pictureBox1.BackgroundImage = Properties.Resources.ok48;
                    textBox1.ReadOnly = true;
                    textBox2.ReadOnly = true;
                    button1.Enabled = false;
                }
                catch { };
               
        }

        private void button5_Click(object sender, EventArgs e) //remove (user)
        {
            var proc1 = new ProcessStartInfo();
            string Command;
            proc1.UseShellExecute = true;
            Command = @"net user " + textBox4.Text + " /delete";
            proc1.WorkingDirectory = @"C:\Windows\System32";
            proc1.FileName = @"C:\Windows\System32\cmd.exe";
            proc1.Verb = "runas";
            proc1.Arguments = "/c " + Command;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc1);
        }

        List<Process> processlist = new List<Process>();
        private void button3_Click(object sender, EventArgs e) // exit
        {
            foreach (var process in Process.GetProcessesByName("cmd"))
            {
                process.Kill();
            }
            this.Close();
        }

        private void button4_Click_1(object sender, EventArgs e) // get user info
        {
            var proc1 = new ProcessStartInfo();
            string Command;
            proc1.UseShellExecute = true;
            Command = @"net user " + textBox3.Text + " && pause";
            proc1.WorkingDirectory = @"C:\Windows\System32";
            proc1.FileName = @"C:\Windows\System32\cmd.exe";
            proc1.Verb = "runas";
            proc1.Arguments = "/c " + Command;
            proc1.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(proc1);
        }

        private void button6_Click(object sender, EventArgs e) // get list of all users
        {
            var proc1 = new ProcessStartInfo();
            string Command;
            proc1.UseShellExecute = true;
            Command = @"net user && pause";
            proc1.WorkingDirectory = @"C:\Windows\System32";
            proc1.FileName = @"C:\Windows\System32\cmd.exe";
            proc1.Verb = "runas";
            proc1.Arguments = "/c " + Command;
            proc1.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(proc1);
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button9_Click(object sender, EventArgs e) //reset
        {
            var proc1 = new ProcessStartInfo();
            string Command;
            proc1.UseShellExecute = true;
            Command = @"net user " + Properties.Settings.Default.user + " /delete";
            proc1.WorkingDirectory = @"C:\Windows\System32";
            proc1.FileName = @"C:\Windows\System32\cmd.exe";
            proc1.Verb = "runas";
            proc1.Arguments = "/c " + Command;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc1);
            textBox1.Text = "";
            textBox2.Text = "";
            Properties.Settings.Default.user = "";
            Properties.Settings.Default.Save();
            Properties.Settings.Default.password = "";
            Properties.Settings.Default.Save();
            pictureBox1.BackgroundImage = Properties.Resources.no48;
            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
            button1.Enabled = true;
        }




        private void button7_Click(object sender, EventArgs e) //launch
        {

            var fileContent = string.Empty;
            var filePath = string.Empty;
            var ressss = string.Empty;

            if (
                Properties.Settings.Default.user == "" && Properties.Settings.Default.password == ""
            )
            {
                MessageBox.Show("Create USER !!!");
            }

            if (Properties.Settings.Default.fifleN == "")
                try
                {
                    VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();

                    dlg.ShowNewFolderButton = true;
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string fifleN = dlg.SelectedPath.Replace("\u005C", "\u002F");
                        Properties.Settings.Default.fifleN = fifleN;
                        Properties.Settings.Default.Save();
                    }
                }
                catch { };

            if (Properties.Settings.Default.exenameV2 == "")
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Exe file (*.exe)|*.exe";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        ressss = Path.GetFileName(filePath);
                        Properties.Settings.Default.exenameV2 = ressss;
                        Properties.Settings.Default.Save();
                    }
                }


            if (Properties.Settings.Default.user != "" && Properties.Settings.Default.fifleN != "" || Properties.Settings.Default.user != "" && Properties.Settings.Default.exenameV2 != "")
              //  try
                {

                    if (checkBox_after.Checked == true && Properties.Settings.Default.third_party_ware_path != "")
                    {
                        var exe = Properties.Settings.Default.third_party_ware_path + "\u002F" + Properties.Settings.Default.third_party_ware_exe;
                        start_sequence();
                        Thread.Sleep(TimeSpan.FromMilliseconds(Properties.Settings.Default.clock));
                        third_party_sequence();
                        //  MessageBox.Show(exe + " started  - after::true");
                    }
                    if (checkBox_before.Checked == true && Properties.Settings.Default.third_party_ware_path != "")
                    {
                        third_party_sequence();
                        //   MessageBox.Show(exe + " started  - before::true");
                        Thread.Sleep(TimeSpan.FromMilliseconds(Properties.Settings.Default.clock));
                        start_sequence();
                    }
                    if (checkBox_after.Checked == false && checkBox_before.Checked == false || Properties.Settings.Default.third_party_ware_path == "")
                    {
                        start_sequence();
                    }

                }
               // catch { };
        }


       // public bool LoadUserProfile { get; set; }
        private void third_party_sequence()
        {
            var exe = Properties.Settings.Default.third_party_ware_path + "\u002F" + Properties.Settings.Default.third_party_ware_exe;
            Directory.SetCurrentDirectory(@Properties.Settings.Default.third_party_ware_path);
            var process = new Process();
            var securePassword = new SecureString();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = exe;
            process.Start();
        }
        private void start_sequence()
        {
            string filePath = Properties.Settings.Default.fifleN;
            string exenameV2 = Properties.Settings.Default.exenameV2;
            string userStr = Properties.Settings.Default.user;
            string userPwd = Properties.Settings.Default.password;
            var exe = filePath + "\u002F" + exenameV2;
            Directory.SetCurrentDirectory(@filePath);
            var process = new Process();
            var securePassword = new SecureString();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = exe;
            process.StartInfo.LoadUserProfile = true;
            process.StartInfo.Verb = "runas";
            process.StartInfo.Domain = System.Environment.UserDomainName;
            process.StartInfo.UserName = userStr;
            var password = userPwd;
            for (int x = 0; x < password.Length; x++)
               securePassword.AppendChar(password[x]);
            process.StartInfo.Password = securePassword;
            process.Start();
            
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            Properties.Settings.Default.fifle = "";
            Properties.Settings.Default.Save();
            Properties.Settings.Default.exename = "";
            Properties.Settings.Default.Save();
            Properties.Settings.Default.fifleN = "";
            Properties.Settings.Default.Save();
            Properties.Settings.Default.exenameV2 = "";
            Properties.Settings.Default.Save();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.third_party_ware_path == "")
                try
                {
                    VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();

                    dlg.ShowNewFolderButton = true;
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string path_o = dlg.SelectedPath.Replace("\u005C", "\u002F");
                        Properties.Settings.Default.third_party_ware_path = path_o;
                        Properties.Settings.Default.Save();
                    }
                }
                catch { };

            if (Properties.Settings.Default.third_party_ware_exe == "")

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    var fileContent = string.Empty;
                    var filePath = string.Empty;
                    var path_f = string.Empty;

                    openFileDialog.Filter = "Exe file (*.exe)|*.exe";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        path_f = Path.GetFileName(filePath);
                        Properties.Settings.Default.third_party_ware_exe = path_f;
                        Properties.Settings.Default.Save();
                    }
                }



            if (Properties.Settings.Default.third_party_ware_path != "" && Properties.Settings.Default.third_party_ware_exe != "")
            {
                textBox5.Text = Properties.Settings.Default.third_party_ware_exe;
            }
            button12.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var num = int.Parse(textBox6.Text);
            Properties.Settings.Default.clock = num;
            Properties.Settings.Default.Save();
            var sec = num / 0x3E8;
            TimeSpan time = TimeSpan.FromSeconds(sec);
            string str = time.ToString(@"mm\:ss");
            label4.Text = str;

            // if then if then if then if then if then if then if then if then if then if then if then if then if then 
            if (checkBox_before.Checked == false && checkBox_after.Checked == false)
            {
                checkBox_restart.Enabled = false;
            }            
            
            if (checkBox_before.Checked == false && checkBox_after.Checked == true)
            { 
                checkBox_restart.Enabled = true;
            }

            if (checkBox_before.Checked == true && checkBox_after.Checked == false )
            {                
                checkBox_restart.Enabled = true;
            }
           
            if (checkBox_restart.Checked == true)
            {
                if (counter >= 100 )
                {
                    string app = Properties.Settings.Default.third_party_ware_exe;
                    var text = app.Replace(".exe", "");

                    Process[] pname = Process.GetProcessesByName(text);
                    if (pname.Length == 0)
                    {
                        var exe = Properties.Settings.Default.third_party_ware_path + "\u002F" + Properties.Settings.Default.third_party_ware_exe;
                        Thread.Sleep(2500);
                        Process.Start(@exe);
                    }
                }
                else
                {
                    counter++;
                }
            }
            if (checkBox_after.Checked == true && checkBox_before.Checked == true)
            {
                Properties.Settings.Default.toggled = "stop";
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.toggled == "stop")
            {
                checkBox_before.Checked = false; checkBox_after.Checked = false;
            }

            if (Properties.Settings.Default.toggled == "before")
            {
                checkBox_before.Checked = true;
            }            
            if(Properties.Settings.Default.toggled == "after")
            {
                checkBox_after.Checked = true;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.third_party_ware_path = "";
            Properties.Settings.Default.Save();
            Properties.Settings.Default.third_party_ware_exe = "";
            Properties.Settings.Default.Save();
            button12.Enabled = true;
            textBox5.Text = "";
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

            Properties.Settings.Default.timer = textBox6.Text;
            Properties.Settings.Default.Save();
        }

        private void checkBox_after_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox_after.Checked == true)
            {
                Properties.Settings.Default.toggled = "after";
                Properties.Settings.Default.Save();
            }      
            else
            {
                Properties.Settings.Default.toggled = "stop";
                Properties.Settings.Default.Save();
            }
        }

        private void checkBox_before_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox_before.Checked == true)
            {
                Properties.Settings.Default.toggled = "before";
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.toggled = "stop";
                Properties.Settings.Default.Save();
            }
        }

     
    }

    public static class OpenFileDialogExtensions
        {
            public static string ShowDialogAndReturnFileName(this OpenFileDialog dialog)
            {
                dialog.ShowDialog();
                return dialog.FileName;
            }
        }
    }
