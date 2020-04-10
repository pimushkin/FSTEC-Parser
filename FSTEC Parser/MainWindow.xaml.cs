using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using Microsoft.Win32;
using PagedList;

namespace FSTEC_Parser
{
    /// <summary>
    /// Interaction logic for  MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private enum TypeOfPagedList
        {
            Short,
            Full
        }

        public static string DataBasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\thrlist.xlsx";
        public static List<Risk> FullListOfRisks = new List<Risk>();
        public static List<ShortRisk> ShortListOfRisks = new List<ShortRisk>();
        private static int _pageNumber = 1;
        private static int _countOfPages;
        private static IPagedList<ShortRisk> _pagedShortList;
        private static IPagedList<Risk> _pagedList;
        private const int PageSize = 25;
        private static TypeOfPagedList _pagedListType;
        private TypeOfPagedList PagedListType
        {
            get => _pagedListType;
            set
            {
                _pagedListType = value;
                ChangeVisibilityDataGrid();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(DataBasePath) || File.Exists(DataBasePath) && new FileInfo(DataBasePath).Length == 0)
            {
                var downloadWindow = new DownloadWindow();
                downloadWindow.ShowDialog();
            }
            if (File.Exists(DataBasePath) && new FileInfo(DataBasePath).Length != 0)
            {
                LoadLocalDataBaseToDataGrid();
            }
        }

        private void ChangeVisibilityDataGrid()
        {
            if (_pagedListType == TypeOfPagedList.Short)
            {
                for (int i = 2; i < DataGrid.Columns.Count; i++)
                {
                    DataGrid.Columns[i].Visibility = Visibility.Hidden;
                }
            }
            else
            {

                for (int i = 2; i < DataGrid.Columns.Count; i++)
                {
                    DataGrid.Columns[i].Visibility = Visibility.Visible;
                }
            }
        }
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(DataBasePath) && new FileInfo(DataBasePath).Length == 0)
            {
                File.Delete(DataBasePath);
            }

            if (!File.Exists(DataBasePath) || File.Exists(DataBasePath) && new FileInfo(DataBasePath).Length == 0)
            {
                ButtonUpdate.IsEnabled = false;
                ButtonDownload.IsEnabled = true;
                ButtonSaveAs.IsEnabled = false;
                MessageBox.Show("База данных отсутствует!");
                return;

            }
            var change = new ComparisonWindow(); 
            if (change.CountOfUpdated == 0)
            {
                MessageBox.Show("База данных успешно обновлена! Изменения отсуствуют.");
            }

            if (change.CountOfUpdated > 0)
            {
                MessageBox.Show($"База данных успешно обновлена! Количество обновленных записей: {change.CountOfUpdated}.");
            }
            if (change.CountOfUpdated != 0 && change.CountOfUpdated != -1)
            {
                change.ShowDialog();
            }
            if (change.CountOfUpdated == -1)
            {
                change.Close();
                ButtonUpdate.IsEnabled = false;
                ButtonDownload.IsEnabled = true;
                return;
            }
            change.Close();
            PagedListType = TypeOfPagedList.Short;
            _pageNumber = 1;
            ButtonPrev.IsEnabled = false;
            ButtonNext.IsEnabled = _countOfPages >= 2;
            _pagedShortList = ShortListOfRisks.ToPagedList(_pageNumber, PageSize);
            _pagedList = FullListOfRisks.ToPagedList(_pageNumber, PageSize);
            DataGrid.ItemsSource = _pagedShortList;
            ButtonChangeViewMode.Content = "ПОКАЗАТЬ ВСЁ";
            _countOfPages = _pagedShortList.PageCount;
            LabelCountOfPages.Content = string.Format($"{_pageNumber}/{_countOfPages}");
        }
        public static List<Risk> EnumerateRisks()
        {
            var risks = new List<Risk>();
            using (var workbook = new XLWorkbook(DataBasePath))
            {
                ShortListOfRisks.Clear();
                var worksheet = workbook.Worksheets.Worksheet(1);
                for (int row = 3; row <= worksheet.RowsUsed().Count(); ++row)
                {
                    var risk = new Risk
                    {
                        ID = worksheet.Cell(row, 1).Value.ToString(),
                        Name = worksheet.Cell(row, 2).Value.ToString(),
                        Description = worksheet.Cell(row, 3).Value.ToString(),
                        SourceOfThreat = worksheet.Cell(row, 4).Value.ToString(),
                        InteractionObject = worksheet.Cell(row, 5).Value.ToString(),
                        ViolationOfConfidentiality = worksheet.Cell(row, 6).Value.ToString(),
                        IntegrityViolation = worksheet.Cell(row, 7).Value.ToString(),
                        ViolationOfAvailability = worksheet.Cell(row, 8).Value.ToString(),
                    };
                    var shortRisk = new ShortRisk
                    {
                        ID = worksheet.Cell(row, 1).Value.ToString(),
                        Name = worksheet.Cell(row, 2).Value.ToString()
                    };
                    risks.Add(risk);
                    ShortListOfRisks.Add(shortRisk);
                }
                _pagedShortList = ShortListOfRisks.ToPagedList(_pageNumber, PageSize);
                _pagedList = risks.ToPagedList(_pageNumber, PageSize);
                _countOfPages = _pagedShortList.PageCount;
            }
            return risks;
        }

        private void ButtonPrev_Click(object sender, RoutedEventArgs e)
        {
            switch (PagedListType)
            {
                case TypeOfPagedList.Short:
                    if (!_pagedShortList.HasPreviousPage) return;
                    _pageNumber--;
                    _pagedShortList = ShortListOfRisks.ToPagedList(_pageNumber, PageSize);
                    DataGrid.ItemsSource = _pagedShortList;
                    
                    ButtonPrev.IsEnabled = _pagedShortList.HasPreviousPage;
                    ButtonNext.IsEnabled = true;
                    LabelCountOfPages.Content = string.Format($"{_pageNumber}/{_countOfPages}");
                    break;
                case TypeOfPagedList.Full:
                    if (!_pagedList.HasPreviousPage) return;
                    _pageNumber--;
                    _pagedList = FullListOfRisks.ToPagedList(_pageNumber, PageSize);
                    DataGrid.ItemsSource = _pagedList;
                    ButtonPrev.IsEnabled = _pagedList.HasPreviousPage;
                    ButtonNext.IsEnabled = true;
                    LabelCountOfPages.Content = string.Format($"{_pageNumber}/{_countOfPages}");
                    break;
            }
            
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            switch (PagedListType)
            {
                case TypeOfPagedList.Short:
                    if (!_pagedShortList.HasNextPage) return;
                    _pageNumber++;
                    _pagedShortList = ShortListOfRisks.ToPagedList(_pageNumber, PageSize);
                    DataGrid.ItemsSource = _pagedShortList;
                    ButtonNext.IsEnabled = _pagedShortList.HasNextPage;

                    ButtonPrev.IsEnabled = true;
                    LabelCountOfPages.Content = string.Format($"{_pageNumber}/{_countOfPages}");
                    break;
                case TypeOfPagedList.Full:
                    if (!_pagedList.HasNextPage) return;
                    _pageNumber++;
                    _pagedList = FullListOfRisks.ToPagedList(_pageNumber, PageSize);
                    DataGrid.ItemsSource = _pagedList;
                    ButtonNext.IsEnabled = _pagedList.HasNextPage;

                    ButtonPrev.IsEnabled = true;
                    LabelCountOfPages.Content = string.Format($"{_pageNumber}/{_countOfPages}");
                    break;
            }
        }

        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(DataBasePath) || File.Exists(DataBasePath) && new FileInfo(DataBasePath).Length == 0)
            {
                var downloadWindow = new DownloadWindow();
                downloadWindow.ShowDialog();
            }
            if (File.Exists(DataBasePath) && new FileInfo(DataBasePath).Length != 0)
            {
                LoadLocalDataBaseToDataGrid();
            }
            
        }

        private void LoadLocalDataBaseToDataGrid()
        {
            try
            {
                FullListOfRisks = EnumerateRisks();
                ButtonPrev.IsEnabled = false;
                if (_countOfPages < 2)
                {
                    ButtonNext.IsEnabled = false;
                }
                else
                {
                    ButtonNext.IsEnabled = true;
                }

                _pageNumber = 1;
                _pagedShortList = ShortListOfRisks.ToPagedList(_pageNumber, PageSize);
                DataGrid.ItemsSource = _pagedShortList;
                ButtonChangeViewMode.Content = "ПОКАЗАТЬ ВСЁ";
                LabelCountOfPages.Content = string.Format($"{_pageNumber}/{_countOfPages}");
                PagedListType = TypeOfPagedList.Short;
                if (!File.Exists(DataBasePath))
                {
                    ButtonUpdate.IsEnabled = false;
                    ButtonPrev.IsEnabled = false;
                    ButtonNext.IsEnabled = false;
                    ButtonChangeViewMode.IsEnabled = false;
                    ButtonDownload.IsEnabled = true;
                    ButtonSaveAs.IsEnabled = false;
                }
                else
                {
                    ButtonUpdate.IsEnabled = true;
                    ButtonNext.IsEnabled = _countOfPages >= 2;
                    ButtonChangeViewMode.IsEnabled = true;
                    ButtonDownload.IsEnabled = false;
                    ButtonSaveAs.IsEnabled = true;
                }
            }
            catch (FileFormatException ex)
            {
                File.Delete(DataBasePath);
                ButtonUpdate.IsEnabled = false;
                ButtonDownload.IsEnabled = true;
                ButtonSaveAs.IsEnabled = false;
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            catch (Exception ex)
            {
                ButtonUpdate.IsEnabled = false;
                ButtonDownload.IsEnabled = true;
                ButtonSaveAs.IsEnabled = false;
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void ButtonChangeViewMode_Click(object sender, RoutedEventArgs e)
        {
            switch (PagedListType)
            {
                case TypeOfPagedList.Short:
                    _pagedList = FullListOfRisks.ToPagedList(_pageNumber, PageSize);
                    DataGrid.ItemsSource = _pagedList;
                    PagedListType = TypeOfPagedList.Full;
                    ButtonChangeViewMode.Content = "ПРЕДПРОСМОТР";
                    break;
                case TypeOfPagedList.Full:
                    _pagedShortList = ShortListOfRisks.ToPagedList(_pageNumber, PageSize);
                    DataGrid.ItemsSource = _pagedShortList;
                    PagedListType = TypeOfPagedList.Short;
                    ButtonChangeViewMode.Content = "ПОКАЗАТЬ ВСЁ";
                    break;
            }
            
        }

        private void ButtonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var tempShortList = new List<ICloneable>();
            var tempList = new List<ICloneable>();
            FullListOfRisks.ForEach((item) =>
            {
                tempList.Add((ICloneable)item.Clone());
            });
            ShortListOfRisks.ForEach((item) =>
            {
                tempShortList.Add((ICloneable)item.Clone());
            });
            try
            {
                var risks = EnumerateRisks();
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Sheet");

                
                worksheet.Cell(1, "A").Value = "Идентификатор угрозы";
                worksheet.Cell(1, "B").Value = "Наименование угрозы";
                worksheet.Cell(1, "C").Value = "Описание угрозы";
                worksheet.Cell(1, "D").Value = "Источник угрозы";
                worksheet.Cell(1, "E").Value = "Объект воздействия угрозы";
                worksheet.Cell(1, "F").Value = "Нарушение конфиденциальности";
                worksheet.Cell(1, "G").Value = "Нарушение целостности";
                worksheet.Cell(1, "H").Value = "Нарушение доступности";
                for (int row = 0; row < risks.Count; row++)
                {
                    worksheet.Cell(row + 2, "A").Value = risks[row].ID;
                    worksheet.Cell(row + 2, "B").Value = risks[row].Name;
                    worksheet.Cell(row + 2, "C").Value = risks[row].Description;
                    worksheet.Cell(row + 2, "D").Value = risks[row].SourceOfThreat;
                    worksheet.Cell(row + 2, "E").Value = risks[row].InteractionObject;
                    worksheet.Cell(row + 2, "F").Value = risks[row].ViolationOfConfidentiality;
                    worksheet.Cell(row + 2, "G").Value = risks[row].IntegrityViolation;
                    worksheet.Cell(row + 2, "H").Value = risks[row].ViolationOfAvailability;
                }
                
                var rngTable = worksheet.Range("A1:H" + (risks.Count + 1));
                rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();
                var dlg = new SaveFileDialog()
                {
                    Filter = "Книга Excel (*.xlsx)|*.xlsx",
                    InitialDirectory = @"c:\"
                };
                if (dlg.ShowDialog() == true)
                {
                    workbook.SaveAs(dlg.FileName);
                }
            }
            catch (FileFormatException ex)
            {
                File.Delete(DataBasePath);
                ButtonUpdate.IsEnabled = false;
                ButtonDownload.IsEnabled = true;
                ButtonSaveAs.IsEnabled = false;
                FullListOfRisks.Clear();
                ShortListOfRisks.Clear();
                tempList.ForEach((item) =>
                {
                    FullListOfRisks.Add((Risk)item.Clone());
                });
                tempShortList.ForEach((item) =>
                {
                    ShortListOfRisks.Add((ShortRisk)item.Clone());
                });
                _pagedShortList = ShortListOfRisks.ToPagedList(_pageNumber, PageSize);
                _pagedList = FullListOfRisks.ToPagedList(_pageNumber, PageSize);
                _countOfPages = _pagedShortList.PageCount;
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            catch (Exception ex)
            {
                ButtonUpdate.IsEnabled = false;
                ButtonDownload.IsEnabled = true;
                ButtonSaveAs.IsEnabled = false;
                FullListOfRisks.Clear();
                ShortListOfRisks.Clear();
                tempList.ForEach((item) =>
                {
                    FullListOfRisks.Add((Risk) item.Clone());
                });
                tempShortList.ForEach((item) =>
                {
                    ShortListOfRisks.Add((ShortRisk) item.Clone());
                });
                _pagedShortList = ShortListOfRisks.ToPagedList(_pageNumber, PageSize);
                _pagedList = FullListOfRisks.ToPagedList(_pageNumber, PageSize);
                _countOfPages = _pagedShortList.PageCount;
                MessageBox.Show("Ошибка: " + ex.Message);
            }

        }
    }
}
