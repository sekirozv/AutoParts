using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using GardensRu.Models;

namespace GardensRu
{
    public partial class PurchaseOrderRequestWindow : Window
    {
        private static readonly Regex IntegerRegex = new Regex(@"^\d+$");
        private static readonly Regex DecimalRegex = new Regex(@"^\d+([,.]\d{0,2})?$");

        private readonly int _productId;

        public int SelectedSupplierId { get; private set; }
        public int Quantity { get; private set; }
        public decimal EstimatedPrice { get; private set; }

        public PurchaseOrderRequestWindow(int productId, string productName)
        {
            InitializeComponent();
            _productId = productId;

            HeaderTextBlock.Text = $"Заявка поставщику: {productName}";
            PriceTextBox.Text = "0";
            LoadSuppliers();
            SuggestQuantity();
        }

        private void LoadSuppliers()
        {
            var suppliers = new List<ReferenceItem>();

            try
            {
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("SELECT SupplierID, SupplierName FROM Suppliers ORDER BY SupplierName", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suppliers.Add(new ReferenceItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }

                SupplierComboBox.ItemsSource = suppliers;
                if (suppliers.Count > 0)
                {
                    SupplierComboBox.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("В справочнике поставщиков нет записей. Добавьте поставщика перед созданием заявки.",
                        "Нет поставщиков", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке поставщиков: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SuggestQuantity()
        {
            try
            {
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("SELECT StockQuantity, MinStockLevel FROM Products WHERE ProductID = @ProductID", connection))
                {
                    command.Parameters.AddWithValue("@ProductID", _productId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var stock = reader.GetInt32(0);
                            var minStock = reader.GetInt32(1);
                            var recommended = Math.Max(minStock * 2 - stock, minStock > 0 ? minStock : 1);
                            recommended = Math.Max(recommended, 1);

                            RecommendationTextBlock.Text = $"Текущий остаток: {stock} шт., порог: {minStock} шт. Рекомендуемая закупка: {recommended} шт.";
                            QuantityTextBox.Text = recommended.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecommendationTextBlock.Text = $"Не удалось получить рекомендации: {ex.Message}";
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;

            if (!(SupplierComboBox.SelectedItem is ReferenceItem supplier))
            {
                ShowValidation("Выберите поставщика.");
                return;
            }

            if (!IntegerRegex.IsMatch(QuantityTextBox.Text.Trim()) || !int.TryParse(QuantityTextBox.Text.Trim(), out var quantity) || quantity <= 0)
            {
                ShowValidation("Количество должно быть целым положительным числом.");
                return;
            }

            if (!DecimalRegex.IsMatch(PriceTextBox.Text.Trim()))
            {
                ShowValidation("Стоимость укажите числом (до двух знаков после запятой).");
                return;
            }

            SelectedSupplierId = supplier.Id;
            Quantity = quantity;
            EstimatedPrice = decimal.Parse(PriceTextBox.Text.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void QuantityTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IntegerRegex.IsMatch(QuantityTextBox.Text.Insert(QuantityTextBox.SelectionStart, e.Text));
        }

        private void PriceTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !DecimalRegex.IsMatch(PriceTextBox.Text.Insert(PriceTextBox.SelectionStart, e.Text));
        }

        private void ShowValidation(string message)
        {
            ValidationMessageTextBlock.Text = message;
            ValidationMessageTextBlock.Visibility = Visibility.Visible;
        }
    }
}

