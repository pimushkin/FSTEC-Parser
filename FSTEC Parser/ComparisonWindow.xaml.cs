using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using static System.Net.WebRequest;

namespace FSTEC_Parser
{
    /// <summary>
    /// Interaction logic for  ComparisonWindow.xaml
    /// </summary>
    public partial class ComparisonWindow
    {
        private readonly List<Risk> _difNewList = new List<Risk>();
        private readonly List<Risk> _difPrevList = new List<Risk>();
        private readonly List<ICloneable> _prevList = new List<ICloneable>();
        public int CountOfUpdated = -1;
        public ComparisonWindow()
        {
            InitializeComponent();

            try
            {
                if (File.Exists(MainWindow.DataBasePath) && new FileInfo(MainWindow.DataBasePath).Length == 0)
                {
                    File.Delete(MainWindow.DataBasePath);
                }

                MainWindow.FullListOfRisks = MainWindow.EnumerateRisks();
                MainWindow.FullListOfRisks.ForEach((item) => { _prevList.Add((ICloneable) item.Clone()); });
                var httpWebRequest = (HttpWebRequest) Create("https://bdu.fstec.ru/files/documents/thrlist.xlsx");

                using ((HttpWebResponse) httpWebRequest.GetResponse())
                {
                }

                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(new Uri("https://bdu.fstec.ru/files/documents/thrlist.xlsx"),
                        MainWindow.DataBasePath);
                }

                var metrics = MainWindow.EnumerateRisks();
                MainWindow.FullListOfRisks = metrics;
                var count = MainWindow.FullListOfRisks.Count > _prevList.Count ? _prevList.Count : MainWindow.FullListOfRisks.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!MainWindow.FullListOfRisks[i].Equals(_prevList[i]))
                    {
                        _difNewList.Add(MainWindow.FullListOfRisks[i]);
                        _difPrevList.Add((Risk) _prevList[i]);
                    }
                }

                for (int i = count; i < MainWindow.FullListOfRisks.Count; i++)
                {
                    _difNewList.Add(MainWindow.FullListOfRisks[i]);
                }

                for (int i = count; i < _prevList.Count; i++)
                {
                    _difPrevList.Add((Risk) _prevList[i]);
                }

                CountOfUpdated = _difPrevList.Count > _difNewList.Count ? _difPrevList.Count : _difNewList.Count;
                if (CountOfUpdated > 0)
                {
                    prevData.ItemsSource = _difPrevList;
                    newData.ItemsSource = _difNewList;
                }
            }
            catch (WebException)
            {
                MessageBox.Show("Произошла ошибка во время загрузки! Возможно, отсутствует соединение с сетью Интернет. Проверьте соединение и повторите попытку.");
                Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка: " + e.Message);
                Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
