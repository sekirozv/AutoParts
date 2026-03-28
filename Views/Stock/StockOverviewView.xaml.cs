using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GardensRu.Views.Stock
{
    public partial class StockOverviewView : UserControl
    {
        private DataTable _productsTable = new DataTable();
        private DataView _productsView;
        private DataTable _movementsTable = new DataTable();
        private int? _selectedProductId;
        private string _selectedProductName;

        public StockOverviewView()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                const string query = @"
                    SELECT ProductID,
                           Article,
                           ProductName,
                           StockQuantity,
                           MinStockLevel,
                           CASE 
                               WHEN StockQuantity <= 0 THEN 'Нет в наличии'
                               WHEN StockQuantity <= MinStockLevel THEN 'Ниже порога'
                               ELSE 'Достаточно'
                           END AS StockStatus,
                           CASE 
                               WHEN StockQuantity > 0 AND StockQuantity <= MinStockLevel THEN 1
                               ELSE 0
                           END AS IsLowStock
                    FROM Products
                    ORDER BY ProductName";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    _productsTable = new DataTable();
                    adapter.Fill(_productsTable);
                    _productsView = new DataView(_productsTable);
                    ProductsDataGrid.ItemsSource = _productsView;
                }

                ApplyFilter();

                if (_selectedProductId.HasValue)
                {
                    ReselectProduct();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке складских остатков: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMovements(int productId)
        {
            try
            {
                const string query = @"
                    SELECT MovementID,
                           MovementDate,
                           MovementType,
                           Quantity,
                           Reference
                    FROM StockMovements
                    WHERE ProductID = @ProductID
                    ORDER BY MovementDate DESC";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@ProductID", productId);
                    _movementsTable = new DataTable();
                    adapter.Fill(_movementsTable);
                    StockMovementsDataGrid.ItemsSource = _movementsTable.DefaultView;
                }

                SelectedProductInfoTextBlock.Text = $"История движения — {_selectedProductName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке движения товара: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            if (_productsView == null)
            {
                return;
            }

            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(StockSearchTextBox.Text))
            {
                var escaped = StockSearchTextBox.Text.Replace("'", "''");
                filters.Add($"(ProductName LIKE '%{escaped}%' OR Article LIKE '%{escaped}%')");
            }

            _productsView.RowFilter = filters.Count == 0 ? string.Empty : string.Join(" AND ", filters);
        }

        private void StockSearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            StockSearchTextBox.Text = string.Empty;
            ApplyFilter();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is DataRowView row)
            {
                _selectedProductId = (int)row["ProductID"];
                _selectedProductName = row["ProductName"].ToString();
                LoadMovements(_selectedProductId.Value);
            }
            else
            {
                _selectedProductId = null;
                _selectedProductName = null;
                StockMovementsDataGrid.ItemsSource = null;
                SelectedProductInfoTextBlock.Text = "Выберите товар из списка слева";
            }
        }

        private void RegisterArrivalButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterMovement("Приход");
        }

        private void RegisterConsumptionButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterMovement("Расход");
        }

        private void RegisterMovement(string movementType)
        {
            if (!_selectedProductId.HasValue)
            {
                MessageBox.Show("Выберите товар для регистрации движения.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new StockMovementWindow(movementType, _selectedProductName);
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                using (var connection = DataBase.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var insertCommand = new SqlCommand(@"
                                INSERT INTO StockMovements (ProductID, MovementType, Quantity, MovementDate, Reference)
                                VALUES (@ProductID, @MovementType, @Quantity, @MovementDate, @Reference)", connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@ProductID", _selectedProductId.Value);
                            insertCommand.Parameters.AddWithValue("@MovementType", movementType);
                            insertCommand.Parameters.AddWithValue("@Quantity", dialog.Quantity);
                            insertCommand.Parameters.AddWithValue("@MovementDate", dialog.MovementDate);
                            insertCommand.Parameters.AddWithValue("@Reference", (object)dialog.Reference ?? DBNull.Value);
                            insertCommand.ExecuteNonQuery();
                        }

                        var delta = movementType == "Приход" ? dialog.Quantity : -dialog.Quantity;

                        using (var updateCommand = new SqlCommand(@"
                                UPDATE Products
                                SET StockQuantity = StockQuantity + @Delta
                                WHERE ProductID = @ProductID", connection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@Delta", delta);
                            updateCommand.Parameters.AddWithValue("@ProductID", _selectedProductId.Value);
                            updateCommand.ExecuteNonQuery();
                        }

                        using (var controlCommand = new SqlCommand(@"
                                SELECT StockQuantity
                                FROM Products
                                WHERE ProductID = @ProductID", connection, transaction))
                        {
                            controlCommand.Parameters.AddWithValue("@ProductID", _selectedProductId.Value);
                            var actualQuantity = Convert.ToInt32(controlCommand.ExecuteScalar());
                            if (actualQuantity < 0)
                            {
                                transaction.Rollback();
                                MessageBox.Show("На складе недостаточно товара для списания.", "Предупреждение",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }

                        transaction.Commit();
                    }
                }

                LoadProducts();
                if (_selectedProductId.HasValue)
                {
                    LoadMovements(_selectedProductId.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации движения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetMinStockButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedProductId.HasValue)
            {
                MessageBox.Show("Выберите товар для изменения порогового остатка.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new MinStockLevelWindow(_selectedProductName);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (var connection = DataBase.Connect())
                    using (var command = new SqlCommand("UPDATE Products SET MinStockLevel = @MinStock WHERE ProductID = @ProductID", connection))
                    {
                        command.Parameters.AddWithValue("@MinStock", dialog.MinStockLevel);
                        command.Parameters.AddWithValue("@ProductID", _selectedProductId.Value);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    LoadProducts();
                    if (_selectedProductId.HasValue)
                    {
                        LoadMovements(_selectedProductId.Value);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении минимального остатка: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CreatePurchaseOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_selectedProductId.HasValue)
            {
                MessageBox.Show("Выберите товар для создания заявки поставщику.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var requestWindow = new PurchaseOrderRequestWindow(_selectedProductId.Value, _selectedProductName);
            if (requestWindow.ShowDialog() == true)
            {
                try
                {
                    using (var connection = DataBase.Connect())
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            int purchaseOrderId;

                            using (var orderCommand = new SqlCommand(@"
                                    INSERT INTO PurchaseOrders (SupplierID, OrderDate, Status)
                                    VALUES (@SupplierID, GETDATE(), 'Создан');
                                    SELECT CAST(SCOPE_IDENTITY() AS INT);", connection, transaction))
                            {
                                orderCommand.Parameters.AddWithValue("@SupplierID", requestWindow.SelectedSupplierId);
                                purchaseOrderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                            }

                            using (var detailCommand = new SqlCommand(@"
                                    INSERT INTO PurchaseOrderDetails (PurchaseOrderID, ProductID, Quantity, UnitPrice)
                                    VALUES (@PurchaseOrderID, @ProductID, @Quantity, @UnitPrice)", connection, transaction))
                            {
                                detailCommand.Parameters.AddWithValue("@PurchaseOrderID", purchaseOrderId);
                                detailCommand.Parameters.AddWithValue("@ProductID", _selectedProductId.Value);
                                detailCommand.Parameters.AddWithValue("@Quantity", requestWindow.Quantity);
                                detailCommand.Parameters.AddWithValue("@UnitPrice", requestWindow.EstimatedPrice);
                                detailCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }

                    MessageBox.Show("Заявка поставщику успешно сформирована.",
                        "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании заявки поставщику: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void ReselectProduct()
        {
            if (!_selectedProductId.HasValue || _productsView == null)
            {
                return;
            }

            var row = _productsView.Cast<DataRowView>()
                .FirstOrDefault(r => (int)r["ProductID"] == _selectedProductId.Value);

            if (row != null)
            {
                ProductsDataGrid.SelectedItem = row;
                ProductsDataGrid.ScrollIntoView(row);
            }
            else
            {
                ProductsDataGrid.SelectedItem = null;
            }
        }
    }
}

