using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;

namespace GardensRu
{
    public partial class CustomerEditorWindow : Window
    {
        private static readonly Regex PhoneRegex = new Regex(@"^[\d\+\-\(\)\s]{5,}$");
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public int? CreatedCustomerId { get; private set; }

        public CustomerEditorWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("Введите ФИО клиента.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!string.IsNullOrWhiteSpace(PhoneTextBox.Text) && !PhoneRegex.IsMatch(PhoneTextBox.Text))
            {
                MessageBox.Show("Введите корректный телефон.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) && !EmailRegex.IsMatch(EmailTextBox.Text))
            {
                MessageBox.Show("Введите корректный email.", "Подсказка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand(@"
                        INSERT INTO Customers (FullName, Phone, Email, Address)
                        VALUES (@FullName, @Phone, @Email, @Address);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                {
                    command.Parameters.AddWithValue("@FullName", FullNameTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? (object)DBNull.Value : PhoneTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(EmailTextBox.Text) ? (object)DBNull.Value : EmailTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(AddressTextBox.Text) ? (object)DBNull.Value : AddressTextBox.Text.Trim());
                    connection.Open();
                    CreatedCustomerId = Convert.ToInt32(command.ExecuteScalar());
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить клиента: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

