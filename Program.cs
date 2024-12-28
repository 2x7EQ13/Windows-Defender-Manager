using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinDefendMan
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            //NotifyIcon notifyIcon = new NotifyIcon
            //{
            //    Icon = Resource1.broken_shield_60, // Set your icon here
            //    Text = "Defender Manager",
            //    Visible = true,
            //    ContextMenu = new ContextMenu(new MenuItem[]
            //    {
            //        new MenuItem("Exit", (s, e) => Application.Exit())
            //    })
            //};
            Form1 form1 = new Form1();
            form1.ShowInTaskbar = false;
            form1.WindowState = FormWindowState.Minimized;
            form1.FormBorderStyle = FormBorderStyle.None;
            //form1.Icon = Resource1.broken_shield_100;
            // Hide the form
            //form1.Load += (sender, e) => form1.Hide();
            Application.Run(form1);
        }
    }
}
