using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace GardensRu
{
    public partial class MinStockLevelWindow : Window
    {
        private static readonly Regex IntegerRegex = new Regex(@"^\d+$");

        public int MinStockLevel { get; private set; }

        public MinStockLevelWindow(string productName)
        {
            InitializeComponent();
            PromptTextBlock.Text = $"Настройка порогового остатка для товара «{productName}»";
            MinStockTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IntegerRegex.IsMatch(MinStockTextBox.Text.Trim()))
            {
                MessageBox.Show("Введите целое неотрицательное число.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MinStockLevel = int.Parse(MinStockTextBox.Text.Trim());
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void MinStockTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IntegerRegex.IsMatch(MinStockTextBox.Text.Insert(MinStockTextBox.SelectionStart, e.Text));
        }
    }
}


