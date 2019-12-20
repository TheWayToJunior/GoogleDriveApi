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
                var imagePath = Environment.CurrentDirectory + @"\Google_Drive_Logo.svg.ico";
                var programmPath = @"D:\С#\API\GoogleDriveApi\GoogleDriveApi\bin\Debug\GoogleDriveApi.exe";

                var key = Registry.ClassesRoot.CreateSubKey(@"Directory\shell\Google Drive");

                key.SetValue("Icon", imagePath);
                key.CreateSubKey("command").SetValue("", programmPath + " \"%1\"");

            };

            pbDeleteProgramm.Click += (s, e) =>
            {
                Registry.ClassesRoot.CreateSubKey(@"Directory\shell\Google Drive").DeleteSubKey("command");
                Registry.ClassesRoot.CreateSubKey(@"Directory\shell").DeleteSubKey("Google Drive");
            };

            pbDelete.Click += (s, e) =>
            {
                string credPath = @"D:\С#\API\GoogleDriveApi\GoogleDriveApi\bin\Debug\token.json";

                try
                {
                    Directory.Delete(credPath, true);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            };

            btnClose.Click += (s, e) => Application.Exit();
        }
    }
}
