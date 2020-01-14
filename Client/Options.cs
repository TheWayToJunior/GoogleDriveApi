using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace Client
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();

            btnStart.Click += (s, e) =>
            {
                var imagePath = AppDomain.CurrentDomain.BaseDirectory + @"Google_Drive_Logo.svg.ico";
                var programmPath = Environment.CurrentDirectory + @"\GoogleDriveApi.exe";

                var key = Registry.ClassesRoot.CreateSubKey(@"Directory\shell\Google Drive");

                key.SetValue("Icon", imagePath);
                key.CreateSubKey("command").SetValue("", programmPath + " \"%1\"");

            };

            btnDelete.Click += (s, e) =>
            {
                Registry.ClassesRoot.CreateSubKey(@"Directory\shell\Google Drive").DeleteSubKey("command");
                Registry.ClassesRoot.CreateSubKey(@"Directory\shell").DeleteSubKey("Google Drive");
            };

            btnDeleteToken.Click += (s, e) =>
            {
                string credPath = Environment.CurrentDirectory + @"\token.json";

                try
                {
                    Directory.Delete(credPath, true);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            btnClose.Click += (s, e) => Application.Exit();
        }
    }
}
