using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GardensRu.Models;

namespace GardensRu.Views.Inventory
{
    public partial class InventoryView : UserControl
    {
        private readonly int _currentUserId;
        private readonly int _currentUserRole;

        private DataTable _productsTable = new DataTable();
        private DataView _productsView;

        private readonly List<ReferenceItem> _categories = new List<ReferenceItem>();
        private readonly List<ReferenceItem> _brands = new List<ReferenceItem>();

        public InventoryView(int currentUserId, int currentUserRole)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            _currentUserRole = currentUserRole;

            AvailabilityComboBox.SelectedIndex = 0;

            LoadReferenceData();
            LoadProducts();
        }

        private void LoadReferenceData()
        {
            try
            {
                _categories.Clear();
                _categories.Add(new ReferenceItem { Id = -1, Name = "Все категории" });

                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _categories.Add(new ReferenceItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                CategoryFilterComboBox.ItemsSource = _categories;
                CategoryFilterComboBox.SelectedIndex = 0;

                _brands.Clear();
                _brands.Add(new ReferenceItem { Id = -1, Name = "Все бренды" });

                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("SELECT BrandID, BrandName FROM Brands ORDER BY BrandName", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _brands.Add(new ReferenceItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                BrandFilterComboBox.ItemsSource = _brands;
                BrandFilterComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить справочники: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                const string query = @"
                    SELECT p.ProductID,
                           p.Article,
                           p.ProductName,
                           ISNULL(b.BrandName, '—') AS BrandName,
                           ISNULL(c.CategoryName, '—') AS CategoryName,
                           p.BrandID,
                           p.CategoryID,
                           p.Price,
                           p.StockQuantity,
                           p.MinStockLevel,
                           p.Description
                    FROM Products p
                    LEFT JOIN Brands b ON b.BrandID = p.BrandID
                    LEFT JOIN Categories c ON c.CategoryID = p.CategoryID";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    _productsTable = new DataTable();
                    adapter.Fill(_productsTable);
                    _productsView = new DataView(_productsTable);
                    ProductsDataGrid.ItemsSource = _productsView;
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ассортимента: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_productsView == null)
            {
                return;
            }

            var filterParts = new List<string>();

            var searchText = SearchTextBox.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var escaped = EscapeLikeValue(searchText);
                filterParts.Add($"(ProductName LIKE '%{escaped}%' OR Article LIKE '%{escaped}%')");
            }

            if (CategoryFilterComboBox.SelectedItem is ReferenceItem category && category.Id > 0)
            {
                filterParts.Add($"CategoryID = {category.Id}");
            }

            if (BrandFilterComboBox.SelectedItem is ReferenceItem brand && brand.Id > 0)
            {
                filterParts.Add($"BrandID = {brand.Id}");
            }

            if (AvailabilityComboBox.SelectedItem is ComboBoxItem availabilityItem)
            {
                switch (availabilityItem.Tag?.ToString())
                {
                    case "InStock":
                        filterParts.Add("StockQuantity > 0");
                        break;
                    case "LowStock":
                        filterParts.Add("(StockQuantity > 0 AND StockQuantity <= MinStockLevel)");
                        break;
                    case "OutOfStock":
                        filterParts.Add("StockQuantity <= 0");
                        break;
                }
            }

            _productsView.RowFilter = filterParts.Count == 0
                ? string.Empty
                : string.Join(" AND ", filterParts);
        }

        private static string EscapeLikeValue(string value)
        {
            return value.Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]").Replace("'", "''");
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void BrandFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void AvailabilityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            CategoryFilterComboBox.SelectedIndex = 0;
            BrandFilterComboBox.SelectedIndex = 0;
            AvailabilityComboBox.SelectedIndex = 0;
            ApplyFilters();
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ProductEditorWindow(_categories.Skip(1).ToList(), _brands.Skip(1).ToList());
            if (editor.ShowDialog() == true)
            {
                LoadReferenceData();
                LoadProducts();
            }
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                var product = new ProductModel
                {
                    ProductId = (int)selectedRow["ProductID"],
                    Article = selectedRow["Article"].ToString(),
                    ProductName = selectedRow["ProductName"].ToString(),
                    BrandId = selectedRow["BrandID"] != DBNull.Value ? (int?)Convert.ToInt32(selectedRow["BrandID"]) : null,
                    CategoryId = selectedRow["CategoryID"] != DBNull.Value ? (int?)Convert.ToInt32(selectedRow["CategoryID"]) : null,
                    Price = Convert.ToDecimal(selectedRow["Price"]),
                    StockQuantity = Convert.ToInt32(selectedRow["StockQuantity"]),
                    MinStockLevel = Convert.ToInt32(selectedRow["MinStockLevel"]),
                    Description = selectedRow["Description"].ToString()
                };

                var editor = new ProductEditorWindow(_categories.Skip(1).ToList(), _brands.Skip(1).ToList(), product);
                if (editor.ShowDialog() == true)
                {
                    LoadReferenceData();
                    LoadProducts();
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                var productId = (int)selectedRow["ProductID"];
                var productName = selectedRow["ProductName"].ToString();

                var confirmation = MessageBox.Show(
                    $"Удалить товар «{productName}»?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmation == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = DataBase.Connect())
                        using (var command = new SqlCommand("DELETE FROM Products WHERE ProductID = @ProductID", connection))
                        {
                            command.Parameters.AddWithValue("@ProductID", productId);
                            connection.Open();
                            command.ExecuteNonQuery();
                        }

                        LoadProducts();
                    }
                    catch (SqlException sqlEx) when (sqlEx.Number == 547)
                    {
                        MessageBox.Show("Невозможно удалить товар, так как существуют связанные записи (заказы, закупки или совместимость).",
                            "Удаление невозможно", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении товара: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}

