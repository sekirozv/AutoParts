using System;
using System.Windows;

namespace GardensRu
{
    public partial class ChangePasswordWindow : Window
    {
        private readonly int userId;

        public ChangePasswordWindow(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewPasswordBox.Password) ||
                NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают или пусты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string query = $@"UPDATE Users SET Password = '{DataBase.HashPassword(NewPasswordBox.Password)}' 
                                WHERE UserID = {userId}";

                DataBase.ExecuteCommand(query);

                MessageBox.Show("Пароль успешно изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении пароля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}