using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GardensRu.Models;

namespace GardensRu
{
    public partial class ProductEditorWindow : Window
    {
        private readonly IList<ReferenceItem> _categories;
        private readonly IList<ReferenceItem> _brands;
        private readonly bool _isEditMode;
        private readonly ProductModel _originalProduct;

        private static readonly Regex IntegerRegex = new Regex(@"^\d+$");
        private static readonly Regex DecimalRegex = new Regex(@"^\d+([,.]\d{0,2})?$");

        public ProductEditorWindow(IList<ReferenceItem> categories, IList<ReferenceItem> brands, ProductModel product = null)
        {
            InitializeComponent();

            _categories = categories ?? new List<ReferenceItem>();
            _brands = brands ?? new List<ReferenceItem>();
            _originalProduct = product;
            _isEditMode = product != null;

            Title = _isEditMode ? "Редактирование товара" : "Добавление товара";
            HeaderTextBlock.Text = _isEditMode ? "Редактирование товара" : "Добавление товара";

            CategoryComboBox.ItemsSource = new List<ReferenceItem>
            {
                new ReferenceItem { Id = 0, Name = "— Не выбрано —" }
            }.Concat(_categories.OrderBy(x => x.Name)).ToList();

            BrandComboBox.ItemsSource = new List<ReferenceItem>
            {
                new ReferenceItem { Id = 0, Name = "— Не выбрано —" }
            }.Concat(_brands.OrderBy(x => x.Name)).ToList();

            CategoryComboBox.SelectedIndex = 0;
            BrandComboBox.SelectedIndex = 0;

            if (_isEditMode)
            {
                PopulateFields(product);
            }
        }

        private void PopulateFields(ProductModel product)
        {
            ArticleTextBox.Text = product.Article;
            NameTextBox.Text = product.ProductName;
            PriceTextBox.Text = product.Price.ToString("N2");
            StockTextBox.Text = product.StockQuantity.ToString();
            MinStockTextBox.Text = product.MinStockLevel.ToString();
            DescriptionTextBox.Text = product.Description;

            if (product.CategoryId.HasValue)
            {
                CategoryComboBox.SelectedValue = product.CategoryId.Value;
            }

            if (product.BrandId.HasValue)
            {
                BrandComboBox.SelectedValue = product.BrandId.Value;
            }
        }

        private bool ValidateInput(out ProductModel model)
        {
            model = null;
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text))
            {
                return ShowValidationMessage("Укажите артикул товара.");
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                return ShowValidationMessage("Введите наименование товара.");
            }

            if (!DecimalRegex.IsMatch(PriceTextBox.Text.Trim()))
            {
                return ShowValidationMessage("Цена должна быть положительным числом (до двух знаков после запятой).");
            }

            if (!IntegerRegex.IsMatch(StockTextBox.Text.Trim()))
            {
                return ShowValidationMessage("Остаток должен быть целым неотрицательным числом.");
            }

            if (!IntegerRegex.IsMatch(MinStockTextBox.Text.Trim()))
            {
                return ShowValidationMessage("Минимальный остаток должен быть целым неотрицательным числом.");
            }

            var price = decimal.Parse(PriceTextBox.Text.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);
            var stock = int.Parse(StockTextBox.Text.Trim(), CultureInfo.InvariantCulture);
            var minStock = int.Parse(MinStockTextBox.Text.Trim(), CultureInfo.InvariantCulture);

            if (minStock > stock)
            {
                return ShowValidationMessage("Минимальный остаток не может превышать текущий остаток.");
            }

            int? categoryId = null;
            if (CategoryComboBox.SelectedValue is int categoryValue && categoryValue > 0)
            {
                categoryId = categoryValue;
            }

            int? brandId = null;
            if (BrandComboBox.SelectedValue is int brandValue && brandValue > 0)
            {
                brandId = brandValue;
            }

            model = new ProductModel
            {
                ProductId = _originalProduct?.ProductId ?? 0,
                Article = ArticleTextBox.Text.Trim(),
                ProductName = NameTextBox.Text.Trim(),
                Price = price,
                StockQuantity = stock,
                MinStockLevel = minStock,
                Description = DescriptionTextBox.Text?.Trim(),
                BrandId = brandId,
                CategoryId = categoryId
            };

            return true;
        }

        private bool ShowValidationMessage(string message)
        {
            ValidationMessageTextBlock.Text = message;
            ValidationMessageTextBlock.Visibility = Visibility.Visible;
            return false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(out var model))
            {
                return;
            }

            try
            {
                using (var connection = DataBase.Connect())
                {
                    connection.Open();

                    if (_isEditMode)
                    {
                        const string updateSql = @"
                            UPDATE Products
                            SET Article = @Article,
                                ProductName = @ProductName,
                                BrandID = @BrandID,
                                CategoryID = @CategoryID,
                                Description = @Description,
                                Price = @Price,
                                StockQuantity = @StockQuantity,
                                MinStockLevel = @MinStockLevel
                            WHERE ProductID = @ProductID";

                        using (var command = new SqlCommand(updateSql, connection))
                        {
                            FillCommandParameters(command, model);
                            command.Parameters.AddWithValue("@ProductID", model.ProductId);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        const string insertSql = @"
                            INSERT INTO Products (Article, ProductName, BrandID, CategoryID, Description, Price, StockQuantity, MinStockLevel)
                            VALUES (@Article, @ProductName, @BrandID, @CategoryID, @Description, @Price, @StockQuantity, @MinStockLevel);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (var command = new SqlCommand(insertSql, connection))
                        {
                            FillCommandParameters(command, model);
                            var newId = command.ExecuteScalar();
                            model.ProductId = Convert.ToInt32(newId);
                        }
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 2627)
            {
                ShowValidationMessage("Товар с таким артикулом уже существует. Укажите уникальный артикул.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void FillCommandParameters(SqlCommand command, ProductModel model)
        {
            command.Parameters.AddWithValue("@Article", model.Article);
            command.Parameters.AddWithValue("@ProductName", model.ProductName);
            command.Parameters.AddWithValue("@BrandID", (object)model.BrandId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CategoryID", (object)model.CategoryId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object)model.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", model.Price);
            command.Parameters.AddWithValue("@StockQuantity", model.StockQuantity);
            command.Parameters.AddWithValue("@MinStockLevel", model.MinStockLevel);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IntegerRegex.IsMatch(((TextBox)sender).Text.Insert(((TextBox)sender).SelectionStart, e.Text));
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var proposed = ((TextBox)sender).Text.Insert(((TextBox)sender).SelectionStart, e.Text);
            e.Handled = !DecimalRegex.IsMatch(proposed);
        }

        private void PriceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(PriceTextBox.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                PriceTextBox.Text = value.ToString("0.00");
            }
        }
    }
}


