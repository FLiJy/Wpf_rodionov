using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace pr14
{
    public partial class SessionPage : Page
    {
        private Sessions _session;
        private List<Button> _selectedSeats = new List<Button>();
        private Dictionary<(int row, int seat), bool> _occupiedCache;

        public SessionPage(int sessionId)
        {
            InitializeComponent();
            LoadSession(sessionId);
        }

        private void LoadSession(int sessionId)
        {
            try
            {
                _session = Core.Context.Sessions
                    .Include("Film")
                    .Include("Hall")
                    .FirstOrDefault(s => s.Id == sessionId);

                if (_session == null)
                {
                    MessageBox.Show("Сеанс не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService.GoBack();
                    return;
                }

                var now = DateTime.Now;
                if (_session.SessionDate < now.Date ||
                   (_session.SessionDate == now.Date && _session.SessionTime < now.TimeOfDay))
                {
                    MessageBox.Show("Этот сеанс уже прошел", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.GoBack();
                    return;
                }

                txtFilmTitle.Text = _session.Films.Title;
                txtDate.Text = _session.SessionDate.ToString("dd.MM.yyyy");
                txtTime.Text = _session.SessionTime.ToString(@"hh\:mm");
                txtHall.Text = $"{_session.Halls.Name} ({_session.Halls.Category})";
                txtPrice.Text = $"{_session.Price:C}";

                LoadOccupiedSeats();
                BuildSeatLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сеанса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOccupiedSeats()
        {
            try
            {
                var occupied = Core.Context.Tickets
                    .Where(t => t.SessionId == _session.Id)
                    .Select(t => new { t.RowNumber, t.SeatNumber })
                    .ToList();

                _occupiedCache = new Dictionary<(int, int), bool>();
                foreach (var o in occupied)
                {
                    _occupiedCache[(o.RowNumber, o.SeatNumber)] = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки занятых мест: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuildSeatLayout()
        {
            seatGrid.Rows = _session.Halls.RowsCount;
            seatGrid.Columns = _session.Halls.SeatsPerRow;

            for (int r = 1; r <= _session.Halls.RowsCount; r++)
            {
                for (int s = 1; s <= _session.Halls.SeatsPerRow; s++)
                {
                    var btn = new Button
                    {
                        Content = $"{s}",
                        Width = 40,
                        Height = 40,
                        Margin = new Thickness(3),
                        FontSize = 12,
                        FontWeight = FontWeights.Bold,
                        Tag = new SeatData { Row = r, SeatNumber = s } // Используем SeatData вместо dynamic
                    };

                    btn.Style = null;
                    btn.Background = Brushes.White;
                    btn.BorderBrush = Brushes.Gray;
                    btn.BorderThickness = new Thickness(1);
                    btn.Cursor = Cursors.Hand;

                    if (_occupiedCache.ContainsKey((r, s)))
                    {
                        btn.Background = Brushes.LightGray;
                        btn.Foreground = Brushes.DarkGray;
                        btn.IsEnabled = false;
                        btn.ToolTip = "Занято";
                    }
                    else
                    {
                        btn.Click += Seat_Click;
                        btn.ToolTip = $"Ряд {r}, место {s}";
                    }

                    seatGrid.Children.Add(btn);
                }
            }
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            if (btn.Background == Brushes.LightBlue)
            {
                btn.Background = Brushes.White;
                btn.BorderBrush = Brushes.Gray;
                _selectedSeats.Remove(btn);
            }
            else
            {
                btn.Background = Brushes.LightBlue;
                btn.BorderBrush = Brushes.Blue;
                _selectedSeats.Add(btn);
            }

            btnBuy.IsEnabled = _selectedSeats.Count > 0;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var btn in _selectedSeats)
            {
                btn.Background = Brushes.White;
                btn.BorderBrush = Brushes.Gray;
            }
            _selectedSeats.Clear();
            btnBuy.IsEnabled = false;
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                var result = MessageBox.Show(
                    "Для покупки билета необходимо авторизоваться. Перейти на страницу входа?",
                    "Требуется авторизация",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    NavigationService.Navigate(new LoginPage());
                }
                return;
            }

            var seatsText = string.Join(", ", _selectedSeats.Select(b =>
            {
                var seatData = b.Tag as SeatData;
                return $"ряд {seatData.Row}, место {seatData.SeatNumber}";
            }));

            var confirm = MessageBox.Show(
                $"Вы уверены, что хотите купить билет(ы) на:\n" +
                $"{_session.Films.Title}\n" +
                $"{_session.SessionDate:dd.MM.yyyy} {_session.SessionTime:hh\\:mm}\n" +
                $"Зал: {_session.Halls.Name}\n\n" +
                $"Выбранные места:\n{seatsText}\n\n" +
                $"Общая стоимость: {_selectedSeats.Count * _session.Price:C}",
                "Подтверждение покупки",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.No) return;

            try
            {
                using (var transaction = Core.Context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var btn in _selectedSeats)
                        {
                            var seatData = btn.Tag as SeatData;

                            var isOccupied = Core.Context.Tickets
                                .Any(t => t.SessionId == _session.Id &&
                                         t.RowNumber == seatData.Row &&
                                         t.SeatNumber == seatData.SeatNumber);

                            if (isOccupied)
                            {
                                transaction.Rollback();
                                MessageBox.Show(
                                    $"Место (ряд {seatData.Row}, место {seatData.SeatNumber}) уже занято. Пожалуйста, выберите другое место.",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            var ticket = new Tickets
                            {
                                UserId = Core.CurrentUser.Id,
                                SessionId = _session.Id,
                                RowNumber = (byte)seatData.Row,
                                SeatNumber = (byte)seatData.SeatNumber,
                                Price = _session.Price,
                                PurchaseDate = DateTime.Now
                            };

                            Core.Context.Tickets.Add(ticket);
                        }

                        Core.Context.SaveChanges();
                        transaction.Commit();

                        MessageBox.Show(
                            $"Успешно куплено {_selectedSeats.Count} билет(ов)!\n" +
                            $"Общая стоимость: {_selectedSeats.Count * _session.Price:C}",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        NavigationService.Navigate(new OrderConfirmPage(_session, _selectedSeats));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при покупке билета: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}