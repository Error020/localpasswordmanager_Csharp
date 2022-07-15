using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private string entry;

        private void Form2_Load(object sender, EventArgs e)
        {
            entry = Form1.editentry;
            string[] contents = entry.Split('|');
            textBox1.Text = contents[0];
            textBox2.Text = contents[1];

        }

        private void dostuff()
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
            }
            else
            {
                if (textBox1.Text.Contains("|") || textBox2.Text.Contains("|"))
                {
                }
                else
                {
                    // addentry

                    string text = "";
                    text = textBox1.Text + "|" + textBox2.Text;
                    File.AppendAllText(@"c:\passwordmanager\passwords.txt", "\n" + text);

                    // deleteentry

                    string line = null;
                    if (File.Exists(@"c:\passwordmanager\bufferfile.txt"))
                    {
                        File.Delete(@"c:\passwordmanager\bufferfile.txt");
                    }
                    File.Create(@"c:\passwordmanager\bufferfile.txt").Close();
                    using (StreamReader reader = new StreamReader(@"c:\passwordmanager\passwords.txt"))
                    {
                        using (StreamWriter writer = new StreamWriter(@"c:\passwordmanager\bufferfile.txt"))
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (String.Compare(line, entry) == 0 || string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                                    continue;

                                writer.WriteLine(line);
                            }
                        }
                    }
                    File.Copy(@"c:\passwordmanager\bufferfile.txt", @"c:\passwordmanager\passwords.txt", true);

                    if (File.Exists(@"c:\passwordmanager\bufferfile.txt"))
                    {
                        File.Delete(@"c:\passwordmanager\bufferfile.txt");
                    }

                    // removeemptylineatend

                    string path = @"c:\passwordmanager\passwords.txt";

                    var tempFileName = Path.GetTempFileName();
                    try
                    {
                        using (var streamReader = new StreamReader(path))
                        using (var streamWriter = new StreamWriter(tempFileName))
                        {
                            string lin;
                            while ((lin = streamReader.ReadLine()) != null)
                            {
                                if (!string.IsNullOrWhiteSpace(lin))
                                    streamWriter.Write(lin);
                                if (!streamReader.EndOfStream)
                                {
                                    streamWriter.Write("\n");
                                }
                            }
                        }
                        File.Copy(tempFileName, path, true);
                    }
                    finally
                    {
                        File.Delete(tempFileName);
                    }
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dostuff();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dostuff();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dostuff();
            }
        }
    }
}
