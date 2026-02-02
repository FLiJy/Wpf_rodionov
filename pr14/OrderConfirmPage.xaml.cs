using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Controls;

namespace pr14
{
    public partial class OrderConfirmPage : Page
    {
        private Sessions _session;
        private List<Button> _selectedSeats;

        public OrderConfirmPage(Sessions session, List<Button> selectedSeats)
        {
            InitializeComponent();
            _session = session;
            _selectedSeats = selectedSeats;
            LoadOrderInfo();
        }

        private void LoadOrderInfo()
        {
            try
            {
                txtFilm.Text = _session.Films.Title;
                txtHall.Text = $"{_session.Halls.Name} ({_session.Halls.Category})";
                txtDateTime.Text = $"{_session.SessionDate:dd.MM.yyyy} {_session.SessionTime:hh\\:mm}";

                var seatsList = _selectedSeats.Select(b =>
                {
                    var seatData = b.Tag as SeatData;
                    return $"ряд {seatData.Row}, место {seatData.SeatNumber}";
                }).ToList();
                txtSeats.Text = string.Join("\n", seatsList);

                txtPrice.Text = $"{_session.Price:C}";
                txtTotal.Text = $"{_selectedSeats.Count * _session.Price:C}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки информации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void btnPersonal_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PersonalPage());
        }
    }
}