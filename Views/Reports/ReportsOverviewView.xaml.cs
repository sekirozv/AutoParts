using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GardensRu.Views.Reports
{
    public partial class ReportsOverviewView : UserControl
    {
        public ReportsOverviewView()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Today.AddDays(-30);
            EndDatePicker.SelectedDate = DateTime.Today;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue &&
                StartDatePicker.SelectedDate > EndDatePicker.SelectedDate)
            {
                StartDatePicker.SelectedDate = EndDatePicker.SelectedDate.Value.AddDays(-7);
            }
        }

        private (DateTime? from, DateTime? to) GetDateRange()
        {
            DateTime? from = null;
            DateTime? to = null;

            if (StartDatePicker.SelectedDate.HasValue)
            {
                from = StartDatePicker.SelectedDate.Value.Date;
            }

            if (EndDatePicker.SelectedDate.HasValue)
            {
                to = EndDatePicker.SelectedDate.Value.Date.AddDays(1).AddTicks(-1);
            }

            return (from, to);
        }

        private void SalesReportButton_Click(object sender, RoutedEventArgs e)
        {
            var (from, to) = GetDateRange();

            const string query = @"
                SELECT FORMAT(CAST(o.OrderDate AS DATE), 'dd.MM.yyyy') AS [Дата],
                       COUNT(*) AS [Количество заказов],
                       SUM(ISNULL(o.TotalAmount, 0)) AS [Сумма продаж]
                FROM Orders o
                WHERE (@From IS NULL OR o.OrderDate >= @From)
                  AND (@To IS NULL OR o.OrderDate <= @To)
                  AND o.Status <> 'Отменен'
                GROUP BY CAST(o.OrderDate AS DATE)
                ORDER BY CAST(o.OrderDate AS DATE)";

            ExecuteReport("Продажи по дням", query,
                new SqlParameter("@From", (object)from ?? DBNull.Value),
                new SqlParameter("@To", (object)to ?? DBNull.Value));
        }

        private void StockReportButton_Click(object sender, RoutedEventArgs e)
        {
            const string query = @"
                SELECT ProductName AS [Товар],
                       Article AS [Артикул],
                       StockQuantity AS [Остаток],
                       MinStockLevel AS [Порог]
                FROM Products
                ORDER BY ProductName";

            ExecuteReport("Текущие остатки на складе", query);
        }

        private void PopularProductsButton_Click(object sender, RoutedEventArgs e)
        {
            var (from, to) = GetDateRange();

            const string query = @"
                SELECT TOP 20 p.ProductName AS [Товар],
                              SUM(od.Quantity) AS [Продано, шт],
                              SUM(od.Quantity * od.UnitPrice) AS [На сумму]
                FROM OrderDetails od
                INNER JOIN Orders o ON o.OrderID = od.OrderID
                INNER JOIN Products p ON p.ProductID = od.ProductID
                WHERE (@From IS NULL OR o.OrderDate >= @From)
                  AND (@To IS NULL OR o.OrderDate <= @To)
                  AND o.Status <> 'Отменен'
                GROUP BY p.ProductName
                ORDER BY [Продано, шт] DESC";

            ExecuteReport("Популярные позиции", query,
                new SqlParameter("@From", (object)from ?? DBNull.Value),
                new SqlParameter("@To", (object)to ?? DBNull.Value));
        }

        private void MovementReportButton_Click(object sender, RoutedEventArgs e)
        {
            var (from, to) = GetDateRange();

            const string query = @"
                SELECT FORMAT(sm.MovementDate, 'dd.MM.yyyy HH:mm') AS [Дата],
                       p.ProductName AS [Товар],
                       sm.MovementType AS [Тип],
                       sm.Quantity AS [Количество],
                       sm.Reference AS [Основание]
                FROM StockMovements sm
                INNER JOIN Products p ON p.ProductID = sm.ProductID
                WHERE (@From IS NULL OR sm.MovementDate >= @From)
                  AND (@To IS NULL OR sm.MovementDate <= @To)
                ORDER BY sm.MovementDate DESC";

            ExecuteReport("Движение товаров", query,
                new SqlParameter("@From", (object)from ?? DBNull.Value),
                new SqlParameter("@To", (object)to ?? DBNull.Value));
        }

        private void ExecuteReport(string title, string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand(sql, connection))
                using (var adapter = new SqlDataAdapter(command))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    var table = new DataTable();
                    adapter.Fill(table);
                    ReportDataGrid.ItemsSource = table.DefaultView;
                    ReportTitleTextBlock.Text = title;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintReportButton_Click(object sender, RoutedEventArgs e)
        {
            var dataView = ReportDataGrid.ItemsSource as DataView;
            if (dataView == null || dataView.Table.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для печати. Сформируйте отчет.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var title = ReportTitleTextBlock.Text;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "Отчет";
            }

            var flowDoc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 12
            };

            flowDoc.Blocks.Add(new Paragraph(new Run(title))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            });

            var table = new Table { CellSpacing = 0 };
            var dt = dataView.Table;

            foreach (DataColumn col in dt.Columns)
            {
                table.Columns.Add(new TableColumn { Width = new GridLength(120) });
            }

            var headerRow = new TableRow();
            foreach (DataColumn col in dt.Columns)
            {
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run(col.ColumnName)))
                    { FontWeight = FontWeights.Bold });
            }
            var rowGroup = new TableRowGroup();
            rowGroup.Rows.Add(headerRow);

            foreach (DataRow row in dt.Rows)
            {
                var detailRow = new TableRow();
                foreach (var item in row.ItemArray)
                {
                    var text = item?.ToString() ?? "";
                    detailRow.Cells.Add(new TableCell(new Paragraph(new Run(text))));
                }
                rowGroup.Rows.Add(detailRow);
            }

            table.RowGroups.Add(rowGroup);
            flowDoc.Blocks.Add(table);

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDoc).DocumentPaginator, title);
            }
        }
    }
}

