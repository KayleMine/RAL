using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RAL
{

    public partial class Wellcum : Form
    {
        RegistryKey localMachine = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
        private string data;
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
        );
        public Wellcum()
        {
            InitializeComponent();
            random_data();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Why? Just WHY? Don't touch anime girl's!");
            Process.Start("https://youtu.be/rS9vLE0JeEE?t=28");
        }
        private void random_data() //generator
        {
            // generate random user data
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            data = finalString;
        }

        private void button1_Click(object sender, EventArgs e) // save data and run launcher
        {
            if (Properties.Settings.Default.user == "" && Properties.Settings.Default.password == "" && data != "" && data != null)
            try
            {
                    Properties.Settings.Default.user = data;
                        Properties.Settings.Default.Save();
                    Properties.Settings.Default.password = data;
                        Properties.Settings.Default.Save();
                    // add random user
                    DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                    DirectoryEntry NewUser = AD.Children.Add(data, "user");
                    NewUser.Invoke("SetPassword", new object[] { data });
                    NewUser.Invoke("Put", new object[] { "Description", "" });
                    NewUser.CommitChanges();
                    DirectoryEntry grp;
                    grp = AD.Children.Find("Users", "group");
                    if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
                }
           catch { };

            localMachine.OpenSubKey("Software\\VasyanWare\\", true).SetValue("1st_launch", "0"); // flag to prevent open that window in future...
            this.Hide();
            Form1 frm = new Form1();
            frm.ShowDialog();
        }

    }
}
