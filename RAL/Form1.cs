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
using RAL.Properties;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Web;
using log4net.Repository.Hierarchy;
using log4net;
using log4net.Config;
namespace RAL
{
    public partial class Form1 : Form
    {
        private int counter;
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        public string NameLog;
        public string Pass;
        public static readonly ILog log = LogManager.GetLogger(typeof(Form1));
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
            log4net.Config.XmlConfigurator.Configure();
            InitializeTimer();
            textBox5.ReadOnly = true;
            if (Properties.Settings.Default.third_party_ware_exe != "")
            {
                textBox5.Text = Properties.Settings.Default.third_party_ware_exe;
                button12.Enabled = false;
            }
            if (Settings.Default.command_line != "" && Settings.Default.command_line != null)
            {
                launch_box.Text = Settings.Default.command_line;
                launch_sbox.Enabled = false;
                launch_box.Enabled = false;
            }
            if (Properties.Settings.Default.timer != "")
            {
                textBox6.Text = Properties.Settings.Default.timer;
            }
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
                catch (Exception error)
                {
                    log.Info(error.Message); MessageBox.Show(error.Message);
                }
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
            SecurityIdentifier everyoneSid = new SecurityIdentifier(
                WellKnownSidType.BuiltinUsersSid,
                null
            );
            string everyone = everyoneSid.Translate(typeof(System.Security.Principal.NTAccount)).ToString();
            string pattern = @"[^\\]*\\";
            string usergroup = Regex.Replace(everyone, pattern, "");
            if (textBox1.Text != "" && textBox2.Text != "" || textBox1.Text != null && textBox2.Text != null  )
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
                    grp = AD.Children.Find(usergroup, "group");
                    if (grp != null)
                        try
                        {
                            grp.Invoke("Add", new object[] { NewUser.Path.ToString() });
                        }
                        catch (Exception error)
                        {
                            log.Info("User groups - Add :: " + error.Message); MessageBox.Show(error.Message);
                        }

                    Settings.Default.user = NameLog;
                    Settings.Default.Save();
                    Settings.Default.password = Pass;
                    Settings.Default.Save();
                    Settings.Default.Upgrade();
                    pictureBox1.BackgroundImage = Properties.Resources.ok48;
                    textBox1.ReadOnly = true;
                    textBox2.ReadOnly = true;
                    button1.Enabled = false;
                }
                catch (Exception error)
                {
                    log.Info("User groups general :: "+error.Message); MessageBox.Show(error.Message);
                }
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
            textBox4.Text = null;
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
            if (Properties.Settings.Default.user == "" && Properties.Settings.Default.password == "")
            {
                MessageBox.Show("Create USER !!!");
                log.Info("Launch :: User not exists.");
            }
            if (Properties.Settings.Default.fifleN == "") try
                {
                    VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                    dlg.ShowNewFolderButton = true;
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) try
                        {
                            string fifleN = dlg.SelectedPath.Replace("\u005C", "\u002F");
                            Properties.Settings.Default.fifleN = fifleN;
                            Properties.Settings.Default.Save();
                        }
                        catch (Exception error)
                        {
                            log.Info("Launch - folder :: " + error.Message);
                            MessageBox.Show(error.Message);
                        }
                }
                catch (Exception error)
                {
                    log.Info("Launch - folder general :: " + error.Message);
                    MessageBox.Show(error.Message);
                }
            if (Properties.Settings.Default.exenameV2 == "") try
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Exe file (*.exe)|*.exe";
                        if (openFileDialog.ShowDialog() == DialogResult.OK) try
                            {
                                //Get the path of specified file
                                filePath = openFileDialog.FileName;
                                ressss = Path.GetFileName(filePath);
                                Properties.Settings.Default.exenameV2 = ressss;
                                Properties.Settings.Default.Save();
                            }
                            catch (Exception error)
                            {
                                log.Info("Launch - executable :: " + error.Message);
                                MessageBox.Show(error.Message);
                            }
                    }
                }
                catch (Exception error)
                {
                    log.Info("Launch - exe general :: " + error.Message);
                    MessageBox.Show(error.Message);
                }
            if (Properties.Settings.Default.user != "" && Properties.Settings.Default.fifleN != "" || Properties.Settings.Default.user != "" && Properties.Settings.Default.exenameV2 != "")
            {
                if (checkBox_after.Checked == true && Properties.Settings.Default.third_party_ware_path != "") try
                    {
                        var exe = Properties.Settings.Default.third_party_ware_path + "\u002F" + Properties.Settings.Default.third_party_ware_exe;
                        start_sequence();
                        Thread.Sleep(TimeSpan.FromMilliseconds(Properties.Settings.Default.clock));
                        third_party_sequence();
                        //  MessageBox.Show(exe + " started  - after::true");
                    }
                    catch (Exception error)
                    {
                        log.Info("Load - Seq. -> 3rd :: " + error.Message);
                        MessageBox.Show(error.Message);
                    }
                if (checkBox_before.Checked == true && Properties.Settings.Default.third_party_ware_path != "") try
                    {
                        third_party_sequence();
                        //   MessageBox.Show(exe + " started  - before::true");
                        Thread.Sleep(TimeSpan.FromMilliseconds(Properties.Settings.Default.clock));
                        start_sequence();
                    }
                    catch (Exception error)
                    {
                        log.Info("Load - Seq. <- 3rd :: " + error.Message);
                        MessageBox.Show(error.Message);
                    }
                if (checkBox_after.Checked == false && checkBox_before.Checked == false || Properties.Settings.Default.third_party_ware_path == "") try
                    {
                        start_sequence();
                    }
                    catch (Exception error)
                    {
                        log.Info("Load - seq. :: " + error.Message);
                        MessageBox.Show(error.Message);
                    }
            }
        }

        // public bool LoadUserProfile { get; set; }
        private void third_party_sequence()
        {
            try
            {
                var exe = Properties.Settings.Default.third_party_ware_path + "\u002F" + Properties.Settings.Default.third_party_ware_exe;
                Directory.SetCurrentDirectory(@Properties.Settings.Default.third_party_ware_path);
                var process = new Process();
                var securePassword = new SecureString();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = exe;
                process.Start();
            }
            catch (Exception error)
            {
                log.Info("Start - 3rd seq. :: " + error.Message);
                MessageBox.Show(error.Message);
            }
        }
        private void start_sequence()
        {
            string commandLineArguments;
            if (Properties.Settings.Default.command_line != "" && Properties.Settings.Default.command_line != null)
            {
                commandLineArguments = String.Format(Properties.Settings.Default.command_line);
            }
            else
            {
                commandLineArguments = "";
            }

            string filePath = Properties.Settings.Default.fifleN;
            string exeNameV2 = Properties.Settings.Default.exenameV2;
            string userName = Properties.Settings.Default.user;
            string userPassword = Properties.Settings.Default.password;

            string fullExePath = Path.Combine(filePath, exeNameV2);

            try
            {
                Directory.SetCurrentDirectory(filePath);

                var process = new Process();
                var securePassword = new SecureString();

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = fullExePath;
                process.StartInfo.LoadUserProfile = true;
                process.StartInfo.Verb = "runas";
                process.StartInfo.Domain = System.Environment.UserDomainName;
                process.StartInfo.UserName = userName;
                process.StartInfo.Arguments = commandLineArguments;

                foreach (char c in userPassword)
                {
                    securePassword.AppendChar(c);
                }

                process.StartInfo.Password = securePassword;
                process.Start();
            }
            catch (Exception error)
            {
                log.Info("Start - seq. :: " + error.Message);
                MessageBox.Show(error.Message);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.fifleN != "")
            {
                Properties.Settings.Default.fifleN = "";
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.exenameV2 != "")
            {
                Properties.Settings.Default.exenameV2 = "";
                Properties.Settings.Default.Save();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Check if the path and executable for the third-party ware have been set
            if (Properties.Settings.Default.third_party_ware_path == "" || Properties.Settings.Default.third_party_ware_exe == "")
            {
                try
                {
                    // Create a VistaFolderBrowserDialog to select the path for the third-party ware
                    VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
                    folderBrowserDialog.ShowNewFolderButton = true;
                    // If the user selects a folder, save the path to the settings
                    if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) try
                        {
                            string path = folderBrowserDialog.SelectedPath.Replace("\u005C", "\u002F");
                            Properties.Settings.Default.third_party_ware_path = path;
                            Properties.Settings.Default.Save();
                        }
                        catch (Exception error)
                        {
                            log.Info("Path - 3rd - folder :: "+error.Message);
                            MessageBox.Show(error.Message);
                        }
                }
                catch (Exception error)
                {
                    log.Info("Path - 3rd - general folder :: " + error.Message);
                    MessageBox.Show(error.Message);
                }

                try
                {
                    // Create an OpenFileDialog to select the executable for the third-party ware
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Exe file (*.exe)|*.exe";

                        // If the user selects a file, save the executable name to the settings
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                            try
                            {
                                string filePath = openFileDialog.FileName;
                                string executableName = Path.GetFileName(filePath);
                                Properties.Settings.Default.third_party_ware_exe = executableName;
                                Properties.Settings.Default.Save();
                            }
                            catch (Exception error)
                            {
                                log.Info("Path - 3rd - exe :: " + error.Message); MessageBox.Show(error.Message);
                            }
                    }
                }
                catch (Exception error)
                {
                    log.Info("Path - 3rd - general exe :: " + error.Message); MessageBox.Show(error.Message);
                }
            }

            // If the path and executable for the third-party ware have been set, display the executable name in the text box
            if (
                Properties.Settings.Default.third_party_ware_path != ""
                && Properties.Settings.Default.third_party_ware_exe != ""
            )
            {
                textBox5.Text = Properties.Settings.Default.third_party_ware_exe;
            }

            // Disable the button once the configuration is complete
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

            if (checkBox_before.Checked == true && checkBox_after.Checked == false)
            {
                checkBox_restart.Enabled = true;
            }

            if (checkBox_restart.Checked == true)
            {
                if (counter >= 100)
                {
                    string app = Properties.Settings.Default.third_party_ware_exe;
                    var text = app.Replace(".exe", "");

                    Process[] pname = Process.GetProcessesByName(text);
                    if (pname.Length == 0)
                    {
                        var exe =
                            Properties.Settings.Default.third_party_ware_path
                            + "\u002F"
                            + Properties.Settings.Default.third_party_ware_exe;
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
                checkBox_before.Checked = false;
                checkBox_after.Checked = false;
            }

            if (Properties.Settings.Default.toggled == "before")
            {
                checkBox_before.Checked = true;
            }
            if (Properties.Settings.Default.toggled == "after")
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

        private void launch_sbox_Click(object sender, EventArgs e)
        {
            if (launch_box.Text != "" && launch_box.Text != " " && launch_box.Text != null)
                try
                {
                    Settings.Default.command_line = launch_box.Text;
                    Settings.Default.Save();
                    launch_sbox.Enabled = false;
                    launch_box.Enabled = false;
                }
                catch (Exception a)
                {
                    MessageBox.Show(a.ToString());
                }
            ;
        }

        private void launchbox_reset_Click(object sender, EventArgs e)
        {
            Settings.Default.command_line = null;
            Settings.Default.Save();
            launch_box.Enabled = true;
            launch_sbox.Enabled = true;
        }

        private void button13_Click(object sender, EventArgs e) // open settings
        {
            if (
                Properties.Settings.Default.user == "" && Properties.Settings.Default.password == ""
            )
            {
                log.Info("User not exists"); MessageBox.Show("Create USER !!!");
            }
            else
            {
                string username = Properties.Settings.Default.user;
                string path = @"C:\Users\" + username + @"\Documents\";
                Process.Start("explorer.exe", path);
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
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
