using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GardensRu.Models;

namespace GardensRu
{
    public partial class OrderEditorWindow : Window
    {
        private readonly ObservableCollection<OrderCartItem> _cartItems = new ObservableCollection<OrderCartItem>();
        private DataTable _productsTable = new DataTable();
        private DataView _productsView;
        private readonly List<ReferenceItem> _customers = new List<ReferenceItem>();

        private static readonly Regex IntegerRegex = new Regex(@"^\d+$");

        public int? CreatedOrderId { get; private set; }

        public OrderEditorWindow()
        {
            InitializeComponent();
            CartDataGrid.ItemsSource = _cartItems;
            CartDataGrid.CellEditEnding += CartDataGrid_CellEditEnding;
            LoadCustomers();
            LoadProducts();
            UpdateSummary();
        }

        private void LoadCustomers()
        {
            try
            {
                _customers.Clear();
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("SELECT CustomerID, FullName FROM Customers ORDER BY FullName", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _customers.Add(new ReferenceItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                CustomerComboBox.ItemsSource = null;
                CustomerComboBox.ItemsSource = _customers;
                if (_customers.Count > 0)
                {
                    CustomerComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                const string query = @"
                    SELECT ProductID,
                           Article,
                           ProductName,
                           Price,
                           StockQuantity
                    FROM Products
                    WHERE StockQuantity > 0
                    ORDER BY ProductName";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    _productsTable = new DataTable();
                    adapter.Fill(_productsTable);
                    _productsView = new DataView(_productsTable);
                    ProductsDataGrid.ItemsSource = _productsView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ассортимента: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyProductFilter()
        {
            if (_productsView == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(ProductSearchTextBox.Text))
            {
                _productsView.RowFilter = string.Empty;
            }
            else
            {
                var search = ProductSearchTextBox.Text.Replace("'", "''");
                _productsView.RowFilter = $"ProductName LIKE '%{search}%' OR Article LIKE '%{search}%'";
            }
        }

        private void ProductSearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyProductFilter();

        private void ResetProductFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ProductSearchTextBox.Text = string.Empty;
            ApplyProductFilter();
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var row = ProductsDataGrid.SelectedItem as DataRowView;
            if (row == null)
            {
                MessageBox.Show("Выберите товар для добавления в заказ.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var productId = (int)row["ProductID"];
            var existing = _cartItems.FirstOrDefault(item => item.ProductId == productId);
            var stock = Convert.ToInt32(row["StockQuantity"]);

            if (existing != null)
            {
                if (existing.Quantity >= existing.MaxQuantity)
                {
                    MessageBox.Show("Нельзя добавить больше, чем доступно на складе.", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                existing.Quantity += 1;
            }
            else
            {
                _cartItems.Add(new OrderCartItem
                {
                    ProductId = productId,
                    Article = row["Article"].ToString(),
                    ProductName = row["ProductName"].ToString(),
                    UnitPrice = Convert.ToDecimal(row["Price"]),
                    MaxQuantity = stock,
                    Quantity = 1
                });
            }

            UpdateSummary();
        }

        private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is OrderCartItem item)
            {
                _cartItems.Remove(item);
                UpdateSummary();
            }
        }

        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (textBox == null)
            {
                e.Handled = true;
                return;
            }

            e.Handled = !IntegerRegex.IsMatch(textBox.Text.Insert(textBox.SelectionStart, e.Text));
        }

        private void CartDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
            {
                return;
            }

            if (e.Column.DisplayIndex == 1)
            {
                var editor = e.EditingElement as System.Windows.Controls.TextBox;
                var item = e.Row.Item as OrderCartItem;
                if (editor == null || item == null)
                {
                    return;
                }

                if (!int.TryParse(editor.Text, out var quantity) || quantity <= 0)
                {
                    quantity = 1;
                }

                if (quantity > item.MaxQuantity)
                {
                    MessageBox.Show($"Максимально доступное количество: {item.MaxQuantity}.", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    quantity = item.MaxQuantity;
                }

                item.Quantity = quantity;
                UpdateSummary();
            }
        }

        private void UpdateSummary()
        {
            var total = _cartItems.Sum(i => i.LineTotal);
            OrderTotalTextBlock.Text = $"Итого: {total:N2} ₽";
            CartInfoTextBlock.Text = $"Товаров в заказе: {_cartItems.Count}";
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var customerWindow = new CustomerEditorWindow();
            if (customerWindow.ShowDialog() == true)
            {
                LoadCustomers();
                if (customerWindow.CreatedCustomerId.HasValue)
                {
                    CustomerComboBox.SelectedValue = customerWindow.CreatedCustomerId.Value;
                }
            }
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var customer = CustomerComboBox.SelectedItem as ReferenceItem;
            if (customer == null)
            {
                MessageBox.Show("Выберите клиента.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в заказ.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_cartItems.Any(ci => ci.Quantity <= 0))
            {
                MessageBox.Show("Количество товаров должно быть больше нуля.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var total = _cartItems.Sum(ci => ci.LineTotal);

            try
            {
                using (var connection = DataBase.Connect())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        int orderId;

                        using (var orderCommand = new SqlCommand(@"
                                INSERT INTO Orders (CustomerID, OrderDate, Status, TotalAmount)
                                VALUES (@CustomerID, GETDATE(), 'Новый', @TotalAmount);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);", connection, transaction))
                        {
                            orderCommand.Parameters.AddWithValue("@CustomerID", customer.Id);
                            orderCommand.Parameters.AddWithValue("@TotalAmount", total);
                            orderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                        }

                        foreach (var item in _cartItems)
                        {
                            using (var detailCommand = new SqlCommand(@"
                                    INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
                                    VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice)", connection, transaction))
                            {
                                detailCommand.Parameters.AddWithValue("@OrderID", orderId);
                                detailCommand.Parameters.AddWithValue("@ProductID", item.ProductId);
                                detailCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                detailCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                                detailCommand.ExecuteNonQuery();
                            }

                            using (var stockUpdateCommand = new SqlCommand(@"
                                    UPDATE Products
                                    SET StockQuantity = StockQuantity - @Quantity
                                    WHERE ProductID = @ProductID", connection, transaction))
                            {
                                stockUpdateCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                stockUpdateCommand.Parameters.AddWithValue("@ProductID", item.ProductId);
                                stockUpdateCommand.ExecuteNonQuery();
                            }

                            using (var stockControlCommand = new SqlCommand(@"
                                    SELECT StockQuantity FROM Products WHERE ProductID = @ProductID", connection, transaction))
                            {
                                stockControlCommand.Parameters.AddWithValue("@ProductID", item.ProductId);
                                var quantityLeft = Convert.ToInt32(stockControlCommand.ExecuteScalar());
                                if (quantityLeft < 0)
                                {
                                    transaction.Rollback();
                                    MessageBox.Show($"Недостаточно товара «{item.ProductName}» на складе.", "Предупреждение",
                                        MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                            }

                            using (var movementCommand = new SqlCommand(@"
                                    INSERT INTO StockMovements (ProductID, MovementType, Quantity, MovementDate, Reference)
                                    VALUES (@ProductID, 'Расход', @Quantity, GETDATE(), @Reference)", connection, transaction))
                            {
                                movementCommand.Parameters.AddWithValue("@ProductID", item.ProductId);
                                movementCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                movementCommand.Parameters.AddWithValue("@Reference", $"Заказ №{orderId}");
                                movementCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        CreatedOrderId = orderId;
                        DialogResult = true;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить заказ: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private class OrderCartItem : INotifyPropertyChanged
        {
            private int _quantity;
            private decimal _lineTotal;

            public int ProductId { get; set; }
            public string Article { get; set; }
            public string ProductName { get; set; }
            public decimal UnitPrice { get; set; }
            public int MaxQuantity { get; set; }

            public int Quantity
            {
                get => _quantity;
                set
                {
                    var newValue = Math.Max(1, Math.Min(value, MaxQuantity));
                    if (_quantity != newValue)
                    {
                        _quantity = newValue;
                        LineTotal = _quantity * UnitPrice;
                        OnPropertyChanged(nameof(Quantity));
                    }
                }
            }

            public decimal LineTotal
            {
                get => _lineTotal;
                private set
                {
                    if (_lineTotal != value)
                    {
                        _lineTotal = value;
                        OnPropertyChanged(nameof(LineTotal));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

