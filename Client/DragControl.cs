using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Project_1PCS_16_
{
    class DragControl : Component
    {
        private Control HendleControl;

        public Control SelectControl
        {
            get
            {
                return this.HendleControl;
            }
            set
            {
                this.HendleControl = value;
                this.HendleControl.MouseDown += new MouseEventHandler(this.DragForm_MouseDown);
            }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr a, int msg, int wParam, int IParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void DragForm_MouseDown(object sender, MouseEventArgs e)
        {
            bool flag = e.Button == MouseButtons.Left;

            if(flag)
            {
                DragControl.ReleaseCapture();
                DragControl.SendMessage(this.SelectControl.FindForm().Handle, 161, 2, 0);
            }
        }
    }
}
