using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;

namespace FSTEC_Parser
{
    /// <summary>
    /// Interaction logic for  DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow
    {
        public DownloadWindow()
        {
            InitializeComponent();
        }
        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(MainWindow.DataBasePath) && new FileInfo(MainWindow.DataBasePath).Length == 0)
                {
                    File.Delete(MainWindow.DataBasePath);
                }
                ButtonConfirm.IsEnabled = false;
                ButtonRefuse.IsEnabled = false;
                using (var webClient = new WebClient())
                {
                    ProgressBar.Visibility = Visibility.Visible;
                    webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                    webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                    webClient.DownloadFileAsync(new Uri("https://bdu.fstec.ru/files/documents/thrlist.xlsx"), MainWindow.DataBasePath);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Ошибка: " + ex.Message);
                Close();
            }
        }

        private void ButtonRefuse_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (ProgressBar.Value == 100)
            {
                MessageBox.Show("Файл базы данных успешно загружен!");
            }
            else
            {
                if (File.Exists(MainWindow.DataBasePath) && new FileInfo(MainWindow.DataBasePath).Length == 0)
                {
                    File.Delete(MainWindow.DataBasePath);
                }
                MessageBox.Show("Произошла ошибка во время загрузки! Возможно, отсутствует соединение с сетью Интернет. Проверьте соединение и повторите попытку.");
            }
            
            Close();
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }
    }
}
