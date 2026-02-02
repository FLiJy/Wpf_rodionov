using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace pr14
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = Core.Context.Users.FirstOrDefault(u => u.Login == login);

                if (user == null)
                {
                    MessageBox.Show("Пользователь с таким логином не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool isValidPassword = false;
                try
                {
                    isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                }
                catch
                {
                    isValidPassword = password == user.PasswordHash;
                }

                if (isValidPassword)
                {
                    Core.CurrentUser = user;
                    MessageBox.Show($"Добро пожаловать, {user.Login}!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {
                        NavigationService.Navigate(new MainPage());
                    }
                }
                else
                {
                    MessageBox.Show("Неверный пароль", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}