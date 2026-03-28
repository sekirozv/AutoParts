using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace GardensRu
{
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
            RoleComboBox.Items.Add("Админ");
            RoleComboBox.Items.Add("Менеджер");
            RoleComboBox.Items.Add("Клиент");
            RoleComboBox.SelectedIndex = 2;

            // обработчики для проверки ввода
            EmailTextBox.LostFocus += ValidateEmail;
            PhoneTextBox.LostFocus += ValidatePhone;
            PhoneTextBox.TextChanged += FormatPhoneNumber;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                int roleId;
                switch (RoleComboBox.SelectedIndex)
                {
                    case 0:
                        roleId = 1; // Админ
                        break;
                    case 1:
                        roleId = 2; // Менеджер
                        break;
                    default:
                        roleId = 3; // Клиент
                        break;
                }

                string query = $@"INSERT INTO Users (Username, Password, RoleID, Email, Phone, FullName) 
                                VALUES ('{UsernameTextBox.Text}', 
                                        '{DataBase.HashPassword(PasswordBox.Password)}', 
                                        {roleId}, 
                                        '{EmailTextBox.Text}', 
                                        {(string.IsNullOrEmpty(PhoneTextBox.Text) ? "NULL" : $"'{PhoneTextBox.Text}'")}, '{FullNameBox.Text}')";

                DataBase.ExecuteCommand(query);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!ValidateEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Введите корректный email (формат: *@gmail.com или *@mail.ru)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(PhoneTextBox.Text) && !ValidatePhone(PhoneTextBox.Text))
            {
                MessageBox.Show("Введите корректный телефон (формат: +7XXXXXXXXXX)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void ValidateEmail(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!ValidateEmail(textBox.Text))
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            }
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@(gmail\.com|mail\.ru)$";
            return Regex.IsMatch(email, pattern);
        }

        private void ValidatePhone(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox.Text) && !ValidatePhone(textBox.Text))
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
            else
            {
                textBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            }
        }

        private bool ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            string pattern = @"^\+7\d{10}$";
            return Regex.IsMatch(phone, pattern);
        }

        private void FormatPhoneNumber(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            string text = textBox.Text.Replace(" ", "").Replace("-", "");

            if (text.Length == 1 && !text.StartsWith("+"))
            {
                text = "+7" + text;
            }
            else if (text.Length > 0 && !text.StartsWith("+7"))
            {
                text = "+7" + text.Substring(1);
            }

            textBox.Text = text;
            textBox.CaretIndex = textBox.Text.Length;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}