using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DaemonNT.Logging;
using XRouter.Common;

namespace XRouter.Gui
{
    /// <summary>
    /// Interaction logic for LogViewControl.xaml
    /// </summary>
    public partial class LogViewControl : UserControl
    {
        private static readonly int PageSize = 30;

        private LogRowsProvider rowsProvider;

        private int currentPage = 1;

        public LogViewControl()
        {
            InitializeComponent();
        }

        internal void Initialize(LogRowsProvider rowsProvider)
        {
            this.rowsProvider = rowsProvider;
        }

        public void UpdateLogs()
        {
            currentPage = 1;
            LoadRows();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (uiLogs.Items.Count == 0) {
                return;
            }
            currentPage++;
            LoadRows();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage == 1) {
                return;
            }
            currentPage--;
            LoadRows();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadRows();
        }

        private void LoadRows()
        {
            DateTime minDate, maxDate = DateTime.MaxValue;
            if (!DateTime.TryParse(uiMinDate.Text, out minDate)) {
                minDate = DateTime.MinValue;
            }
            if (!DateTime.TryParse(uiMaxDate.Text, out maxDate)) {
                maxDate = DateTime.MaxValue;
            }

            LogLevelFilters levelFilter = LogLevelFilters.None;
            if (uiFilterInfo.IsChecked == true) {
                levelFilter |= LogLevelFilters.Info;
            }
            if (uiFilterWarning.IsChecked == true) {
                levelFilter |= LogLevelFilters.Warning;
            }
            if (uiFilterError.IsChecked == true) {
                levelFilter |= LogLevelFilters.Error;
            }

            LogRow[] rows = rowsProvider(minDate, maxDate, levelFilter, currentPage, PageSize);
            uiLogs.ItemsSource = rows;
            uiPageNumber.Text = currentPage.ToString();
        }

        internal delegate LogRow[] LogRowsProvider(DateTime minDate, DateTime maxDate, LogLevelFilters levelFilter, int pageNumber, int pageSize);

        internal class LogRow
        {
            public string Created { get; private set; }

            public string LogType { get; private set; }

            public string Message { get; private set; }

            public LogRow(DateTime created, LogType logType, string message)
            {
                Created = created.ToString();
                LogType = logType.ToString();
                Message = message.Replace(Environment.NewLine, " ");
            }
        }
    }
}
