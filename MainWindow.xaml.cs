using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace GardensRu
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "SELECT UserID, Username, Password, RoleID FROM Users WHERE Username = @Username";
                DataTable dt = new DataTable();

                using (var connection = DataBase.Connect())
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", LoginTextBox.Text.Trim());
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }


                if (dt.Rows.Count > 0)
                {
                    string dbPasswordHash = dt.Rows[0]["Password"].ToString();
                    string inputPassword = ShowPasswordCheckBox.IsChecked == true ? VisiblePasswordBox.Text : PasswordBox.Password;
                    string inputPasswordHash = DataBase.HashPassword(inputPassword);

                    if (inputPasswordHash == dbPasswordHash)
                    {
                        int userId = Convert.ToInt32(dt.Rows[0]["UserID"]);
                        string username = dt.Rows[0]["Username"].ToString();
                        int roleId = Convert.ToInt32(dt.Rows[0]["RoleID"]);


                        switch (roleId)
                        {
                            case 1: // Администратор
                            case 2: // Менеджер
                                Panel panel = new Panel(userId, username, roleId);
                                panel.Show();
                                this.Close();
                                break;
                            case 3:
                                MessageBox.Show("Клиентам доступ в программу воспрещен!", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            default:
                                MessageBox.Show("Неизвестная роль пользователя", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VisiblePasswordBox.Text = PasswordBox.Password;
            VisiblePasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = VisiblePasswordBox.Text;
            PasswordBox.Visibility = Visibility.Visible;
            VisiblePasswordBox.Visibility = Visibility.Collapsed;
        }
    }
}