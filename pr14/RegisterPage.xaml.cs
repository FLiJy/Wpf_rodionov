using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace pr14
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var email = txtEmail.Text.Trim();
            var phone = txtPhone.Text.Trim();
            var password = txtPassword.Password;
            var passwordConfirm = txtPasswordConfirm.Password;

            if (string.IsNullOrWhiteSpace(login) || login.Length < 3)
            {
                MessageBox.Show("Логин должен содержать не менее 3 символов", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != passwordConfirm)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (Core.Context.Users.Any(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Core.Context.Users.Any(u => u.Email == email))
                {
                    MessageBox.Show("Пользователь с таким email уже существует", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string passwordHash;
                try
                {
                    passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                }
                catch
                {
                    passwordHash = password;
                }

                var user = new Users
                {
                    Login = login,
                    PasswordHash = passwordHash,
                    Email = email,
                    Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                    RegisterDate = DateTime.Now
                };

                Core.Context.Users.Add(user);
                Core.Context.SaveChanges();

                Core.CurrentUser = user;
                MessageBox.Show($"Регистрация успешна! Добро пожаловать, {user.Login}!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new MainPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}