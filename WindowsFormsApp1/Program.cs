using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            void findpaths()
            {
                if (!Directory.Exists(@"c:\passwordmanager"))
                {
                    Directory.CreateDirectory(@"c:\passwordmanager");
                }
                if (!File.Exists(@"c:\passwordmanager\loginstate.txt"))
                {
                    File.Create(@"c:\passwordmanager\loginstate.txt").Close();
                    File.WriteAllText(@"c:\passwordmanager\loginstate.txt", "0");       // 0 == not logged in | 1 == logged in
                }
                if (!File.Exists(@"c:\passwordmanager\masterpassword.txt"))
                {
                    File.Create(@"c:\passwordmanager\masterpassword.txt").Close();
                    File.WriteAllText(@"c:\passwordmanager\masterpassword.txt", "1337");
                }
                if (!File.Exists(@"c:\passwordmanager\passwords.txt"))
                {
                    File.Create(@"c:\passwordmanager\passwords.txt").Close();
                }
            }
            findpaths();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
