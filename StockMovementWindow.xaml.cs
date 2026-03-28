using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace GardensRu
{
    public partial class StockMovementWindow : Window
    {
        private static readonly Regex IntegerRegex = new Regex(@"^\d+$");
        private readonly string _movementType;

        public int Quantity { get; private set; }
        public DateTime MovementDate { get; private set; }
        public string Reference { get; private set; }

        public StockMovementWindow(string movementType, string productName)
        {
            InitializeComponent();

            _movementType = movementType;
            TitleTextBlock.Text = $"{movementType} товара";
            ProductTextBlock.Text = productName;

            MovementDatePicker.SelectedDate = DateTime.Today;
            MovementTimeTextBox.Text = DateTime.Now.ToString("HH:mm");
            QuantityTextBox.Focus();
        }

        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IntegerRegex.IsMatch(QuantityTextBox.Text.Insert(QuantityTextBox.SelectionStart, e.Text));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;

            if (!IntegerRegex.IsMatch(QuantityTextBox.Text.Trim()) || !int.TryParse(QuantityTextBox.Text.Trim(), out var quantity) || quantity <= 0)
            {
                ShowValidationMessage("Количество должно быть целым положительным числом.");
                return;
            }

            if (MovementDatePicker.SelectedDate == null)
            {
                ShowValidationMessage("Укажите дату движения.");
                return;
            }

            var timeText = MovementTimeTextBox.Text.Trim();
            if (!DateTime.TryParseExact(timeText, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            {
                ShowValidationMessage("Укажите время в формате ЧЧ:ММ.");
                return;
            }

            Quantity = quantity;
            MovementDate = MovementDatePicker.SelectedDate.Value.Date.AddHours(time.Hour).AddMinutes(time.Minute);
            Reference = string.IsNullOrWhiteSpace(ReferenceTextBox.Text) ? $"{_movementType} (ручная запись)" : ReferenceTextBox.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowValidationMessage(string message)
        {
            ValidationMessageTextBlock.Text = message;
            ValidationMessageTextBlock.Visibility = Visibility.Visible;
        }
    }
}


