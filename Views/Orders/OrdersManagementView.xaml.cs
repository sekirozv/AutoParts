using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using GardensRu.Models;

namespace GardensRu.Views.Orders
{
    public partial class OrdersManagementView : UserControl
    {
        private readonly int _currentUserId;

        private DataTable _ordersTable = new DataTable();
        private DataView _ordersView;
        private DataTable _orderDetailsTable = new DataTable();
        private int? _selectedOrderId;

        public OrdersManagementView(int currentUserId)
        {
            _currentUserId = currentUserId;
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                const string query = @"
                    SELECT o.OrderID,
                           o.OrderDate,
                           o.Status,
                           c.FullName AS CustomerName,
                           ISNULL(o.TotalAmount, SUM(od.Quantity * od.UnitPrice)) AS TotalAmount
                    FROM Orders o
                    INNER JOIN Customers c ON c.CustomerID = o.CustomerID
                    LEFT JOIN OrderDetails od ON od.OrderID = o.OrderID
                    GROUP BY o.OrderID, o.OrderDate, o.Status, c.FullName, o.TotalAmount
                    ORDER BY o.OrderDate DESC, o.OrderID DESC";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    _ordersTable = new DataTable();
                    adapter.Fill(_ordersTable);
                    _ordersView = new DataView(_ordersTable);
                    OrdersDataGrid.ItemsSource = _ordersView;
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrderDetails(int orderId)
        {
            try
            {
                const string query = @"
                    SELECT od.OrderDetailID,
                           p.ProductName,
                           od.Quantity,
                           od.UnitPrice,
                           (od.Quantity * od.UnitPrice) AS LineTotal
                    FROM OrderDetails od
                    INNER JOIN Products p ON p.ProductID = od.ProductID
                    WHERE od.OrderID = @OrderID";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@OrderID", orderId);
                    _orderDetailsTable = new DataTable();
                    adapter.Fill(_orderDetailsTable);
                    OrderDetailsDataGrid.ItemsSource = _orderDetailsTable.DefaultView;
                }

                var orderRow = _ordersTable.AsEnumerable()
                    .FirstOrDefault(r => r.Field<int>("OrderID") == orderId);

                if (orderRow != null)
                {
                    OrderSummaryTextBlock.Text = $"Заказ №{orderRow["OrderID"]}, клиент: {orderRow["CustomerName"]}, статус: {orderRow["Status"]}, сумма: {Convert.ToDecimal(orderRow["TotalAmount"]):N2} ₽";
                    StatusUpdateComboBox.SelectedItem = StatusUpdateComboBox.Items
                        .Cast<ComboBoxItem>()
                        .FirstOrDefault(item => Equals(item.Content?.ToString(), orderRow["Status"].ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке состава заказа: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_ordersView == null)
            {
                return;
            }

            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(OrderSearchTextBox.Text))
            {
                var search = OrderSearchTextBox.Text.Replace("'", "''");
                filters.Add($"(Convert(OrderID, 'System.String') LIKE '%{search}%' OR CustomerName LIKE '%{search}%')");
            }

            if (StatusFilterComboBox.SelectedItem is ComboBoxItem statusItem && statusItem.Tag?.ToString() != "All")
            {
                var status = statusItem.Tag.ToString().Replace("'", "''");
                filters.Add($"Status = '{status}'");
            }

            if (StartDatePicker.SelectedDate.HasValue)
            {
                filters.Add($"OrderDate >= #{StartDatePicker.SelectedDate.Value:MM/dd/yyyy}#");
            }

            if (EndDatePicker.SelectedDate.HasValue)
            {
                filters.Add($"OrderDate <= #{EndDatePicker.SelectedDate.Value:MM/dd/yyyy}#");
            }

            _ordersView.RowFilter = filters.Count == 0 ? string.Empty : string.Join(" AND ", filters);
        }

        private void OrderSearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            OrderSearchTextBox.Text = string.Empty;
            StatusFilterComboBox.SelectedIndex = 0;
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            ApplyFilters();
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is DataRowView row)
            {
                _selectedOrderId = (int)row["OrderID"];
                LoadOrderDetails(_selectedOrderId.Value);
            }
            else
            {
                _selectedOrderId = null;
                OrderDetailsDataGrid.ItemsSource = null;
                OrderSummaryTextBlock.Text = "Выберите заказ для просмотра подробностей.";
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
            OrderDetailsDataGrid.ItemsSource = null;
            OrderSummaryTextBlock.Text = "Выберите заказ для просмотра подробностей.";
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = new OrderEditorWindow();
            if (editor.ShowDialog() == true)
            {
                LoadOrders();
                if (editor.CreatedOrderId.HasValue)
                {
                    SelectOrder(editor.CreatedOrderId.Value);
                }
            }
        }

        private void UpdateStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedOrderId.HasValue)
            {
                MessageBox.Show("Выберите заказ для изменения статуса.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!(StatusUpdateComboBox.SelectedItem is ComboBoxItem item))
            {
                MessageBox.Show("Выберите новый статус.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newStatus = item.Content.ToString();
            var orderIdToSelect = _selectedOrderId.Value;

            try
            {
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("UPDATE Orders SET Status = @Status WHERE OrderID = @OrderID", connection))
                {
                    command.Parameters.AddWithValue("@Status", newStatus);
                    command.Parameters.AddWithValue("@OrderID", orderIdToSelect);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                LoadOrders();
                SelectOrder(orderIdToSelect);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении статуса: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument("Счет на оплату");
        }

        private void PrintSlipButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument("Товарная накладная");
        }

        private void PrintDocument(string documentTitle)
        {
            if (!_selectedOrderId.HasValue)
            {
                MessageBox.Show("Выберите заказ для печати документа.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var orderRow = OrdersDataGrid.SelectedItem as DataRowView;
            if (orderRow == null)
            {
                return;
            }

            var flowDoc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 12
            };

            flowDoc.Blocks.Add(new Paragraph(new Run(documentTitle))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            });

            flowDoc.Blocks.Add(new Paragraph(new Run($"Заказ №{orderRow["OrderID"]} от {Convert.ToDateTime(orderRow["OrderDate"]):dd.MM.yyyy}"))
            {
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 12, 0, 0)
            });

            flowDoc.Blocks.Add(new Paragraph(new Run($"Клиент: {orderRow["CustomerName"]}")));
            flowDoc.Blocks.Add(new Paragraph(new Run($"Статус: {orderRow["Status"]}")));

            var table = new Table { CellSpacing = 0 };
            table.Columns.Add(new TableColumn { Width = new GridLength(240) });
            table.Columns.Add(new TableColumn { Width = new GridLength(60) });
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });

            var headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Товар"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Кол-во"))) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Цена"))) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сумма"))) { FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Right });

            var rowGroup = new TableRowGroup();
            rowGroup.Rows.Add(headerRow);

            foreach (DataRow detail in _orderDetailsTable.Rows)
            {
                var detailRow = new TableRow();
                detailRow.Cells.Add(new TableCell(new Paragraph(new Run(detail["ProductName"].ToString()))));
                detailRow.Cells.Add(new TableCell(new Paragraph(new Run(detail["Quantity"].ToString()))) { TextAlignment = TextAlignment.Center });
                detailRow.Cells.Add(new TableCell(new Paragraph(new Run($"{Convert.ToDecimal(detail["UnitPrice"]):N2}"))) { TextAlignment = TextAlignment.Right });
                detailRow.Cells.Add(new TableCell(new Paragraph(new Run($"{Convert.ToDecimal(detail["LineTotal"]):N2}"))) { TextAlignment = TextAlignment.Right });
                rowGroup.Rows.Add(detailRow);
            }

            table.RowGroups.Add(rowGroup);
            flowDoc.Blocks.Add(table);

            flowDoc.Blocks.Add(new Paragraph(new Run($"Итого: {Convert.ToDecimal(orderRow["TotalAmount"]):N2} ₽"))
            {
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            });

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDoc).DocumentPaginator, documentTitle);
            }
        }

        private void SelectOrder(int orderId)
        {
            var row = _ordersView?.Cast<DataRowView>()
                .FirstOrDefault(r => (int)r["OrderID"] == orderId);

            if (row != null)
            {
                OrdersDataGrid.SelectedItem = row;
                OrdersDataGrid.ScrollIntoView(row);
            }
        }
    }
}

