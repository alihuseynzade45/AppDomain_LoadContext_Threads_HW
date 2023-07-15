using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppDomain_LoadContext_Threads_HW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Thread Thread { get; set; }
     
        public MainWindow()
        {
            InitializeComponent();
        }

        public void CopyFile(string from, string to)
        {
            if (File.Exists(from))
            {
                Thread thread = new Thread(() =>
                {
                    using (FileStream fsread = new FileStream(from, FileMode.Open, FileAccess.Read))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar.Maximum = fsread.Length;
                        });

                        using (FileStream fswrite = new FileStream(to, FileMode.Create, FileAccess.Write))
                        {
                            int len = 1;
                            var filesize = fsread.Length;
                            byte[] buffer = new byte[len];
                            int size = 0;
                            do
                            {
                                len = fsread.Read(buffer, 0, buffer.Length);
                                fswrite.Write(buffer, 0, len);
                                filesize -= len;
                                size += len;
                                Dispatcher.Invoke(() =>
                                {
                                    ProgressBar.Value = size;
                                });
                                Thread.Sleep(1);
                                fswrite.Flush();
                            } while (len != 0);

                            Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Copy Succesful");
                                ProgressBar.Value = 0;
                                Thread = null;
                                TBFrom.Text = default;
                                TBTo.Text = default;
                                
                            });
                        }
                    }
                });
                Thread = thread;
                Thread.IsBackground = true;
                Thread.Start();
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (Thread == null)
            {
                CopyFile(TBFrom.Text, TBTo.Text);
            }
        }
        [Obsolete]

        private void FromOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "(*.txt)|*.txt";
            fileDialog.ShowDialog();
            if (fileDialog.FileName != "" && Thread == null)
            {
                TBFrom.Text = fileDialog.FileName;
            }
        }

        private void ToOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "(*.txt)|*.txt";
            file.ShowDialog();
            if (file.FileName != null && Thread == null && file.FileName != TBFrom.Text)
            {
                TBTo.Text = file.FileName;
            }
        }
    }
}