using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GardensRu
{
    public partial class Panel : Window, INotifyPropertyChanged
    {
        private readonly Dictionary<string, UserControl> _viewCache = new Dictionary<string, UserControl>();
        private readonly List<Button> _navigationButtons = new List<Button>();
        private readonly DispatcherTimer _timer;

        private string _currentDateTime;
        private string _currentUserFullName;
        private string _currentUserRoleName;
        private readonly string _currentUserLogin;
        private readonly int _currentUserId;
        private readonly int _currentUserRole;

        public string CurrentDateTime
        {
            get => _currentDateTime;
            set
            {
                _currentDateTime = value;
                OnPropertyChanged(nameof(CurrentDateTime));
            }
        }

        public string CurrentUserFullName
        {
            get => _currentUserFullName;
            set
            {
                _currentUserFullName = value;
                OnPropertyChanged(nameof(CurrentUserFullName));
            }
        }

        public string CurrentUserRoleName
        {
            get => _currentUserRoleName;
            set
            {
                _currentUserRoleName = value;
                OnPropertyChanged(nameof(CurrentUserRoleName));
            }
        }

        public Panel(int userId, string userLogin, int userRole)
        {
            InitializeComponent();
            DataContext = this;

            _currentUserId = userId;
            _currentUserLogin = userLogin;
            _currentUserRole = userRole;

            InitializeNavigationButtons();
            LoadUserProfile();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, _) => UpdateDateTime();
            _timer.Start();

            UpdateDateTime();
            NavigateToInventory();
        }

        private void InitializeNavigationButtons()
        {
            _navigationButtons.AddRange(new[]
            {
                InventoryButton,
                StockButton,
                OrdersButton,
                PurchasesButton,
                ReportsButton,
                UsersButton
            });

            if (_currentUserRole != 1)
            {
                UsersButton.Visibility = Visibility.Collapsed;
                _navigationButtons.Remove(UsersButton);
            }
        }

        private void LoadUserProfile()
        {
            try
            {
                const string query = @"SELECT ISNULL(Users.FullName, Users.Username) AS FullName,
                                              Roles.RoleName
                                       FROM Users
                                       INNER JOIN Roles ON Roles.RoleID = Users.RoleID
                                       WHERE Users.UserID = @UserID";

                using (var connection = DataBase.Connect())
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", _currentUserId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrentUserFullName = reader["FullName"]?.ToString() ?? _currentUserLogin;
                            CurrentUserRoleName = reader["RoleName"]?.ToString() ?? "Сотрудник";
                        }
                        else
                        {
                            CurrentUserFullName = _currentUserLogin;
                            CurrentUserRoleName = "Сотрудник";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentUserFullName = _currentUserLogin;
                CurrentUserRoleName = "Сотрудник";
                MessageBox.Show($"Не удалось загрузить профиль пользователя: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateDateTime()
        {
            CurrentDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        private void SetActiveNavigation(Button activeButton)
        {
            foreach (var button in _navigationButtons)
            {
                button.Tag = Equals(button, activeButton) ? "Active" : null;
            }
        }

        private void Navigate(string key, Func<UserControl> viewFactory, Button activeButton)
        {
            if (!_viewCache.TryGetValue(key, out var view) || view == null)
            {
                view = viewFactory();
                _viewCache[key] = view;
            }

            ContentPresenter.Content = view;
            SetActiveNavigation(activeButton);
        }

        private void NavigateToInventory()
        {
            Navigate("Inventory", () => new Views.Inventory.InventoryView(_currentUserId, _currentUserRole), InventoryButton);
        }

        private void NavigateToStock()
        {
            Navigate("Stock", () => new Views.Stock.StockOverviewView(), StockButton);
        }

        private void NavigateToOrders()
        {
            Navigate("Orders", () => new Views.Orders.OrdersManagementView(_currentUserId), OrdersButton);
        }

        private void NavigateToPurchases()
        {
            Navigate("Purchases", () => new Views.Purchases.PurchaseManagementView(), PurchasesButton);
        }

        private void NavigateToReports()
        {
            Navigate("Reports", () => new Views.Reports.ReportsOverviewView(), ReportsButton);
        }

        private void NavigateToUsers()
        {
            Navigate("Users", () => new Views.Users.UsersManagementView(_currentUserId), UsersButton);
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e) => NavigateToInventory();
        private void StockButton_Click(object sender, RoutedEventArgs e) => NavigateToStock();
        private void OrdersButton_Click(object sender, RoutedEventArgs e) => NavigateToOrders();
        private void PurchasesButton_Click(object sender, RoutedEventArgs e) => NavigateToPurchases();
        private void ReportsButton_Click(object sender, RoutedEventArgs e) => NavigateToReports();
        private void UsersButton_Click(object sender, RoutedEventArgs e) => NavigateToUsers();

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            var authWindow = new MainWindow();
            authWindow.Show();
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}