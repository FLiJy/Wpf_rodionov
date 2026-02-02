using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace pr14
{
    public partial class FilmPage : Page
    {
        private Films _film;

        public FilmPage(int filmId)
        {
            InitializeComponent();
            LoadFilm(filmId);
        }

        private void LoadFilm(int filmId)
        {
            try
            {
                _film = Core.Context.Films
                    .Include("FilmGenres")
                    .Include("FilmGenres.Genre")
                    .FirstOrDefault(f => f.Id == filmId);

                if (_film == null)
                {
                    MessageBox.Show("Фильм не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService.GoBack();
                    return;
                }

                txtTitle.Text = _film.Title;
                txtStartDate.Text = $"Дата выхода: {_film.StartDate:dd.MM.yyyy}";
                txtAgeRating.Text = _film.AgeRating.ToString();
                txtStartDate.Text = $"Дата выхода: {_film.StartDate:dd.MM.yyyy}";   
                txtDescription.Text = _film.Description ?? "Описание отсутствует";

                var genres = _film.Genres.Select(fg => fg.Genres.Name).ToList();
                txtGenres.Text = $"Жанры: {string.Join(", ", genres)}";

                try
                {
                    imgPoster.Source = new BitmapImage(new Uri(_film.PosterUrl, UriKind.Relative));
                }
                catch
                {
                    imgPoster.Source = new BitmapImage(new Uri("/Images/default.jpg", UriKind.Relative));
                }

                LoadSessions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильма: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSessions()
        {
            try
            {
                var today = DateTime.Today;
                var sessions = Core.Context.Sessions
                    .Include("Hall")
                    .Where(s => s.FilmId == _film.Id &&
                               (s.SessionDate > today ||
                               (s.SessionDate == today && s.SessionTime > DateTime.Now.TimeOfDay)))
                    .OrderBy(s => s.SessionDate)
                    .ThenBy(s => s.SessionTime)
                    .ToList();

                sessionsList.ItemsSource = sessions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сеансов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelectSeat_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int sessionId)
            {
                NavigationService.Navigate(new SessionPage(sessionId));
            }
        }
    }
}