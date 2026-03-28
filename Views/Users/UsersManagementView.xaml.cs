using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GardensRu.Views.Users
{
    public partial class UsersManagementView : UserControl
    {
        private readonly int _currentUserId;
        private readonly Dictionary<int, string> _roles = new Dictionary<int, string>();

        public UsersManagementView(int currentUserId)
        {
            _currentUserId = currentUserId;
            InitializeComponent();
            LoadRoles();
            LoadUsers();
        }

        private void LoadRoles()
        {
            try
            {
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("SELECT RoleID, RoleName FROM Roles", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        _roles.Clear();
                        while (reader.Read())
                        {
                            _roles[reader.GetInt32(0)] = reader.GetString(1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUsers()
        {
            try
            {
                const string query = @"
                    SELECT u.UserID,
                           u.Username,
                           u.FullName,
                           u.Email,
                           u.Phone,
                           r.RoleID,
                           r.RoleName
                    FROM Users u
                    INNER JOIN Roles r ON r.RoleID = u.RoleID
                    ORDER BY u.Username";

                using (var adapter = new SqlDataAdapter(query, DataBase.Connect()))
                {
                    var table = new DataTable();
                    adapter.Fill(table);

                    var list = table.AsEnumerable().Select(row => new UserRow
                    {
                        UserID = row.Field<int>("UserID"),
                        Username = row.Field<string>("Username"),
                        FullName = row.Field<string>("FullName"),
                        Email = row.Field<string>("Email"),
                        Phone = row.Field<string>("Phone"),
                        RoleID = row.Field<int>("RoleID"),
                        RoleName = row.Field<string>("RoleName")
                    }).ToList();

                    UsersDataGrid.ItemsSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadUsers();

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddUserWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is UserRow user)
            {
                var changeWindow = new ChangePasswordWindow(user.UserID);
                changeWindow.ShowDialog();
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is UserRow user)
            {
                if (user.UserID == _currentUserId)
                {
                    MessageBox.Show("Нельзя удалить свою учетную запись.", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (IsLastAdministrator(user))
                {
                    MessageBox.Show("Нельзя удалить последнего администратора системы.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show($"Удалить пользователя «{user.Username}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = DataBase.Connect())
                        using (var command = new SqlCommand("DELETE FROM Users WHERE UserID = @UserID", connection))
                        {
                            command.Parameters.AddWithValue("@UserID", user.UserID);
                            connection.Open();
                            command.ExecuteNonQuery();
                        }

                        LoadUsers();
                        MessageBox.Show("Пользователь удален.", "Успешно",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool IsLastAdministrator(UserRow user)
        {
            var isAdminRole = false;

            if (_roles.TryGetValue(user.RoleID, out var roleName))
            {
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    var normalized = roleName.ToLowerInvariant();
                    if (normalized.Contains("админ") || normalized.Contains("admin"))
                    {
                        isAdminRole = true;
                    }
                }
            }

            if (!isAdminRole && user.RoleID != 1)
            {
                return false;
            }

            try
            {
                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE RoleID = @RoleID", connection))
                {
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);
                    connection.Open();
                    var count = (int)command.ExecuteScalar();
                    return count <= 1;
                }
            }
            catch
            {
                return false;
            }
        }

        private class UserRow
        {
            public int UserID { get; set; }
            public string Username { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public int RoleID { get; set; }
            public string RoleName { get; set; }
        }
    }
}

