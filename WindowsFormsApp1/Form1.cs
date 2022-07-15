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

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string loginstate = File.ReadAllText(@"c:\passwordmanager\loginstate.txt");
        string currentpassword = File.ReadAllText(@"c:\passwordmanager\masterpassword.txt");
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!?\\/%&$§@#~+.,;:_[]";

        public static string editentry;

        public void deleteentry(string entry)
        {
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

            removeEmptyLineAtEnd(@"c:\passwordmanager\passwords.txt");

            if (File.Exists(@"c:\passwordmanager\bufferfile.txt"))
            {
                File.Delete(@"c:\passwordmanager\bufferfile.txt");
            }
            updatelb();
        }

        private void removeEmptyLineAtEnd(string path)
        {
            var tempFileName = Path.GetTempFileName();
            try
            {
                using (var streamReader = new StreamReader(path))
                using (var streamWriter = new StreamWriter(tempFileName))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            streamWriter.Write(line);
                        if (!streamReader.EndOfStream)  {
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
        }

        private void login()
        {
            File.WriteAllText(@"c:\passwordmanager\loginstate.txt", "1");
            loginstate = "1";
            label2.Text = "welcome! current masterpassword: " + File.ReadAllText(@"c:\passwordmanager\masterpassword.txt");

            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;

            this.ActiveControl = textBox2;

            updatelb();
        }

        private void logout()
        {
            File.WriteAllText(@"c:\passwordmanager\loginstate.txt", "0");
            loginstate = "0";
            label2.Text = "please log in!";
            listBox1.Items.Clear();

            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
        }
        public class ListItem
        {
            public string service { get; set; }
            public string password { get; set; }

        }

        private void updatelb()
        {
            removeEmptyLineAtEnd(@"c:\passwordmanager\passwords.txt");
            listBox1.Items.Clear();
            string[] lines = File.ReadAllLines(@"c:\passwordmanager\passwords.txt");
            foreach (string line in lines)
            {
                string[] contents = line.Split('|');
                listBox1.Items.Add(new ListItem { service = contents[0], password = contents[1] });
            }
        }

        private void searchupdate(string searched)
        {
            removeEmptyLineAtEnd(@"c:\passwordmanager\passwords.txt");
            listBox1.Items.Clear();
            string[] lines = File.ReadAllLines(@"c:\passwordmanager\passwords.txt");
            foreach (string line in lines)
            {
                string[] contents = line.Split('|');
                if ((contents[0].ToLower()).Contains(searched.ToLower()))
                {
                    listBox1.Items.Add(new ListItem { service = contents[0], password = contents[1] });
                }
            }
        }

        public string CreatePassword(int length)
        {
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (loginstate == "1")
            {
                label2.Text = "already logged in!";
            }
            else if (loginstate == "0")
            {
                if ((textBox1.Text == File.ReadAllText(@"c:\passwordmanager\masterpassword.txt") && !textBox1.Text.Contains(" ") && textBox1.Text != ""))
                {
                    login();
                };
            };
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox1.Text != " ")
            {
                File.WriteAllText(@"c:\passwordmanager\masterpassword.txt", textBox1.Text.ToString());
                label2.Text = "password was changed to: " + File.ReadAllText(@"c:\passwordmanager\masterpassword.txt");
                textBox1.Clear();
            }
            else
            {
                label2.Text = "the textbox is empty or contains spaces. current password: " + File.ReadAllText(@"c:\passwordmanager\masterpassword.txt");
                textBox1.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            logout();
            label2.Text = "logged out... please log in!";
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            if (loginstate == "0")
            {
                logout();
                label2.Text = "please log in...";
            }
            else if (loginstate == "1")
            {
                login();
                comboBox2.SelectedIndex = 1;

            }
            else
            {
                logout();
                label2.Text = "an error occured, please log in...";
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var password = ((ListItem)listBox1.SelectedItem).password;
            System.Windows.Forms.Clipboard.SetText(password);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            searchupdate(textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            searchupdate(textBox2.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int selectedLength;
            if (comboBox2.SelectedIndex == 0) selectedLength = 8;
            else if (comboBox2.SelectedIndex == 1) selectedLength = 12;
            else if (comboBox2.SelectedIndex == 2) selectedLength = 16;
            else if (comboBox2.SelectedIndex == 3) selectedLength = 24;
            else if (comboBox2.SelectedIndex == 4) selectedLength = 32;
            else selectedLength = 8;

            string password = CreatePassword(selectedLength);
            textBox5.Text = password;
            System.Windows.Forms.Clipboard.SetText(password);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "")
            {
                label11.Visible = true;
                label11.Text = "all fields are required.";
            }
            else
            {
                if (textBox5.Text.Contains("|") || textBox4.Text.Contains("|") || textBox3.Text.Contains("|"))
                {
                    label11.Text = "don't use this character '|' !!!!!!";
                    label11.Visible = true;
                }
                else
                {
                    label11.Visible = false;
                    string text = "";
                    text = textBox3.Text + "/" + textBox4.Text + "|" + textBox5.Text;
                    File.AppendAllText(@"c:\passwordmanager\passwords.txt", "\n" + text);
                    updatelb();

                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
             if (listBox1.SelectedIndex != -1)
            {
                var password = ((ListItem)listBox1.SelectedItem).password;
                var service = ((ListItem)listBox1.SelectedItem).service;

                string entry = service + "|" + password;

                deleteentry(entry);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                var password = ((ListItem)listBox1.SelectedItem).password;
                var service = ((ListItem)listBox1.SelectedItem).service;

                editentry = service + "|" + password;

                Form2 form2 = new Form2();
                form2.ShowDialog();
                this.Close();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (loginstate == "1")
                {
                    label2.Text = "already logged in!";
                }
                else if (loginstate == "0")
                {
                    if ((textBox1.Text == File.ReadAllText(@"c:\passwordmanager\masterpassword.txt") && !textBox1.Text.Contains(" ") && textBox1.Text != ""))
                    {
                        login();
                    };
                };
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox4.Focus();
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox5.Focus();
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "")
                {
                    label11.Visible = true;
                    label11.Text = "all fields are required.";
                }
                else
                {
                    if (textBox5.Text.Contains("|") || textBox4.Text.Contains("|") || textBox3.Text.Contains("|"))
                    {
                        label11.Text = "don't use this character '|' !!!!!!";
                        label11.Visible = true;
                    }
                    else
                    {
                        label11.Visible = false;
                        string text = "";
                        text = textBox3.Text + "/" + textBox4.Text + "|" + textBox5.Text;
                        File.AppendAllText(@"c:\passwordmanager\passwords.txt", "\n" + text);
                        updatelb();

                    }
                }
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!(listBox1.Items.Count == 0))
                {
                    listBox1.SelectedIndex = 0;
                    if (listBox1.SelectedIndex != -1)
                    {
                        var password = ((ListItem)listBox1.SelectedItem).password;
                        System.Windows.Forms.Clipboard.SetText(password);
                    }
                }
            }
        }
    }
}
