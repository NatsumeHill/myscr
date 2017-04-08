using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace myscr
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();

            FileStream fileStream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "myscr.ini", FileMode.OpenOrCreate);
            StreamReader streamReader = new StreamReader(fileStream);
            string line = streamReader.ReadLine();
            if (line == null)
                path.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            else
                path.Text = line;
            streamReader.Close();
            fileStream.Close();
        }

        private void Browser_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel) return;
          
            path.Text = dialog.SelectedPath.Trim();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            FileStream config = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "myscr.ini", FileMode.Create);
            byte[] data = Encoding.Default.GetBytes(path.Text.Trim());
            config.Write(data,0,data.Length);
            config.Flush();
            config.Close();
            this.Hide();
            Application.Current.Shutdown();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
