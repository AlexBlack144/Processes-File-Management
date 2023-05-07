using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;


namespace ExamTask
{
    public partial class Form1 : Form
    {
        FolderBrowserDialog folderBrowserDialog;
        RegistryKey key;
        RegistryKey myProgKey;

        public Form1()
        {
            InitializeComponent();
            folderBrowserDialog = new FolderBrowserDialog();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonDelete.Enabled = false;
            textBoxFilter.Enabled = false;

            key = Registry.CurrentUser;
            myProgKey = key.CreateSubKey("MyProg");

            //add key
            myProgKey.SetValue("license", "admin");
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string filter = textBoxFilter.Text;
                ScanList(filter);
            }
            catch (Exception) { }

        }


        #region Buttons
        private void button_Click_Folder(object sender, EventArgs e)
        {
            OpenFolder();
        }

        private void button_Click_Scan(object sender, EventArgs e)
        {

            try
            {
                ScanList();
            }
            catch (Exception) { }
        }

        private void button_Click_Delete(object sender, EventArgs e)
        {
            try
            {
                if (listView1.Items.Count>0)
                {
                    string message = "\tAre you sure, you want to delete \n\t   this file from your PC?";
                    string title = "Delete file";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result = MessageBox.Show(message, title, buttons);
                    if (result == DialogResult.Yes)
                    {
                        string fileToKill = listView1.SelectedItems[0].SubItems[4].Text;
                        DeleteFile(fileToKill.ToString());
                        ScanList();
                    }
                }

            }
            catch (Exception) { }           
        }

        private void button_Click_Paid(object sender, EventArgs e)
        {
            string key = Interaction.InputBox("Key: admin", "Enter the license key!");
            try
            {
                //get key
                var data = myProgKey.GetValue("license");

                if (key == data.ToString())
                {
                    buttonDelete.Enabled = true;
                    textBoxFilter.Enabled = true;
                }
            }
            catch (Exception) { }
        }
        #endregion


        #region Methods
        static long GetFileSize(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                return new FileInfo(FilePath).Length;
            }
            return 0;
        }
        private void ScanList()
        {

            listView1.Items.Clear();

            int id_index = 0;
            string path = textBoxPath.Text;

            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                id_index++;
                long fileSizeibBytes = GetFileSize(file.FullName);
                long fileSizeibMbs = fileSizeibBytes / (1024 * 1024);

                string[] row = new string[] { id_index.ToString(), file.LastWriteTime.ToString(), fileSizeibMbs.ToString() + " Mb", file.Name, file.FullName };

                listView1.Items.Add(new ListViewItem(row));
            }
        }
        private void ScanList(string filter)
        {
            try
            {
                listView1.Items.Clear();

                int id_index = 0;
                string path = textBoxPath.Text;

                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles("*." + filter);
                foreach (FileInfo file in files)
                {

                    id_index++;
                    long fileSizeibBytes = GetFileSize(file.FullName);
                    long fileSizeibMbs = fileSizeibBytes / (1024 * 1024);

                    string[] row = new string[] { id_index.ToString(), file.LastWriteTime.ToString(), fileSizeibMbs.ToString() + " Mb", file.Name, file.FullName };

                    listView1.Items.Add(new ListViewItem(row));
                }
            }
            catch (Exception) { }
        }

        private void OpenFolder()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string filetext = folderBrowserDialog.SelectedPath;
                textBoxPath.Text = filetext;
            }
        }
        private void DeleteFile(string path)
        {
            File.Delete(path);
        }

        #endregion
    }
}
