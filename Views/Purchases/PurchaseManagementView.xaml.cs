using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GardensRu.Views.Purchases
{
    public partial class PurchaseManagementView : UserControl
    {
        private DataTable _ordersTable = new DataTable();
        private DataTable _detailsTable = new DataTable();
        private int? _selectedOrderId;

        public PurchaseManagementView()
        {
            InitializeComponent();
            LoadPurchaseOrders();
        }

        private void LoadPurchaseOrders()
        {
            try
            {
                const string query = @"
                    SELECT po.PurchaseOrderID,
                           po.OrderDate,
                           po.Status,
                           s.SupplierName,
                           ISNULL(SUM(pod.Quantity * pod.UnitPrice), 0) AS TotalAmount
                    FROM PurchaseOrders po
                    INNER JOIN Suppliers s ON s.SupplierID = po.SupplierID
                    LEFT JOIN PurchaseOrderDetails pod ON pod.PurchaseOrderID = po.PurchaseOrderID
                    GROUP BY po.PurchaseOrderID, po.OrderDate, po.Status, s.SupplierName
                    ORDER BY po.OrderDate DESC, po.PurchaseOrderID DESC";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    _ordersTable = new DataTable();
                    adapter.Fill(_ordersTable);
                    PurchaseOrdersDataGrid.ItemsSource = _ordersTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке закупок: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPurchaseDetails(int purchaseOrderId)
        {
            try
            {
                const string query = @"
                    SELECT pod.PurchaseOrderDetailID,
                           p.ProductName,
                           pod.Quantity,
                           pod.UnitPrice,
                           (pod.Quantity * pod.UnitPrice) AS LineTotal,
                           pod.ProductID
                    FROM PurchaseOrderDetails pod
                    INNER JOIN Products p ON p.ProductID = pod.ProductID
                    WHERE pod.PurchaseOrderID = @PurchaseOrderID";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderId);
                    _detailsTable = new DataTable();
                    adapter.Fill(_detailsTable);
                    PurchaseDetailsDataGrid.ItemsSource = _detailsTable.DefaultView;
                }

                var orderRow = _ordersTable.AsEnumerable()
                    .FirstOrDefault(r => r.Field<int>("PurchaseOrderID") == purchaseOrderId);

                if (orderRow != null)
                {
                    PurchaseSummaryTextBlock.Text = $"Закупка №{orderRow["PurchaseOrderID"]}, поставщик: {orderRow["SupplierName"]}, статус: {orderRow["Status"]}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке деталей закупки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PurchaseOrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = PurchaseOrdersDataGrid.SelectedItem as DataRowView;
            if (row != null)
            {
                _selectedOrderId = row.Row.Field<int>("PurchaseOrderID");
                LoadPurchaseDetails(_selectedOrderId.Value);
                return;
            }

            _selectedOrderId = null;
            PurchaseDetailsDataGrid.ItemsSource = null;
            PurchaseSummaryTextBlock.Text = "Выберите заказ поставщику";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPurchaseOrders();
            PurchaseDetailsDataGrid.ItemsSource = null;
            PurchaseSummaryTextBlock.Text = "Выберите заказ поставщику";
        }

        private void MarkInTransitButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("В пути");
        }

        private void MarkReceivedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedOrderId.HasValue)
            {
                MessageBox.Show("Выберите заказ для обновления статуса.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var statusRow = PurchaseOrdersDataGrid.SelectedItem as DataRowView;
            if (statusRow != null)
            {
                var currentStatus = statusRow["Status"]?.ToString();
                if (string.Equals(currentStatus, "Получен", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Заказ уже отмечен как полученный.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            try
            {
                using (var connection = DataBase.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (DataRow row in _detailsTable.Rows)
                        {
                            var productId = row.Field<int>("ProductID");
                            var quantity = row.Field<int>("Quantity");

                            using (var updateStock = new SqlCommand(@"
                                    UPDATE Products
                                    SET StockQuantity = StockQuantity + @Quantity
                                    WHERE ProductID = @ProductID", connection, transaction))
                            {
                                updateStock.Parameters.AddWithValue("@Quantity", quantity);
                                updateStock.Parameters.AddWithValue("@ProductID", productId);
                                updateStock.ExecuteNonQuery();
                            }

                            using (var movementCommand = new SqlCommand(@"
                                    INSERT INTO StockMovements (ProductID, MovementType, Quantity, MovementDate, Reference)
                                    VALUES (@ProductID, 'Приход', @Quantity, GETDATE(), @Reference)", connection, transaction))
                            {
                                movementCommand.Parameters.AddWithValue("@ProductID", productId);
                                movementCommand.Parameters.AddWithValue("@Quantity", quantity);
                                movementCommand.Parameters.AddWithValue("@Reference", $"Закупка №{_selectedOrderId.Value}");
                                movementCommand.ExecuteNonQuery();
                            }
                        }

                        using (var updateStatus = new SqlCommand("UPDATE PurchaseOrders SET Status = 'Получен' WHERE PurchaseOrderID = @PurchaseOrderID", connection, transaction))
                        {
                            updateStatus.Parameters.AddWithValue("@PurchaseOrderID", _selectedOrderId.Value);
                            updateStatus.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                LoadPurchaseOrders();
                if (_selectedOrderId.HasValue)
                {
                    LoadPurchaseDetails(_selectedOrderId.Value);
                }

                MessageBox.Show("Закупка отмечена как полученная, остатки обновлены.", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке поступления: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Отменен");
        }

        private void UpdateStatus(string status)
        {
            if (!_selectedOrderId.HasValue)
            {
                MessageBox.Show("Выберите заказ для обновления статуса.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("UPDATE PurchaseOrders SET Status = @Status WHERE PurchaseOrderID = @PurchaseOrderID", connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@PurchaseOrderID", _selectedOrderId.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                LoadPurchaseOrders();
                if (_selectedOrderId.HasValue)
                {
                    LoadPurchaseDetails(_selectedOrderId.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

