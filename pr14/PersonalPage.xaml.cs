using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pr14
{
    public partial class PersonalPage : Page
    {
        public PersonalPage()
        {
            InitializeComponent();
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            try
            {
                if (Core.CurrentUser == null)
                {
                    MessageBox.Show("Вы не авторизованы", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NavigationService.Navigate(new LoginPage());
                    return;
                }

                txtLogin.Text = Core.CurrentUser.Login;
                txtEmail.Text = Core.CurrentUser.Email;
                txtPhone.Text = string.IsNullOrWhiteSpace(Core.CurrentUser.Phone)
                    ? "Не указан" : Core.CurrentUser.Phone;
                txtRegisterDate.Text = Core.CurrentUser.RegisterDate?.ToString("dd.MM.yyyy HH:mm")
                    ?? "Неизвестно";

                LoadTickets();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTickets()
        {
            try
            {
                var tickets = Core.Context.Tickets
                    .Include("Session")
                    .Include("Session.Film")
                    .Include("Session.Hall")
                    .Where(t => t.UserId == Core.CurrentUser.Id)
                    .OrderByDescending(t => t.PurchaseDate)
                    .ToList();

                if (tickets.Count == 0)
                {
                    ticketsList.Visibility = Visibility.Collapsed;
                    var tb = new TextBlock
                    {
                        Text = "У вас пока нет купленных билетов",
                        FontSize = 18,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 30, 0, 0)
                    };
                    var parent = ticketsList.Parent as Panel;
                    if (parent != null)
                    {
                        parent.Children.Add(tb);
                    }
                }
                else
                {
                    ticketsList.ItemsSource = tickets;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки билетов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите выйти из аккаунта?",
                "Выход",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Core.CurrentUser = null;
                MessageBox.Show("Вы успешно вышли из аккаунта", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new MainPage());
            }
        }
    }
}