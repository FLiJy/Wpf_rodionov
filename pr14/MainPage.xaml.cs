using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace pr14
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            LoadFilms();
        }

        private void LoadFilms(string search = "", string sortTag = "title_asc")
        {
            try
            {
                var query = Core.Context.Films.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    query = query.Where(f => f.Title.ToLower().Contains(search));
                }

                switch (sortTag)
                {
                    case "title_asc":
                        query = query.OrderBy(f => f.Title);
                        break;
                    case "title_desc":
                        query = query.OrderByDescending(f => f.Title);
                        break;
                    case "rating_desc":
                        query = query.OrderByDescending(f => f.Rating);
                        break;
                    case "rating_asc":
                        query = query.OrderBy(f => f.Rating);
                        break;
                }

                filmsGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильмов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var sortTag = (cmbSort.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "title_asc";
            LoadFilms(txtSearch.Text, sortTag);
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sortTag = (cmbSort.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "title_asc";
            LoadFilms(txtSearch.Text, sortTag);
        }

        private void filmsGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as DependencyObject;
            while (element != null && !(element is ContentPresenter))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element is ContentPresenter presenter && presenter.Content is Films film)
            {
                NavigationService.Navigate(new FilmPage(film.Id));
            }
        }

        private void btnPersonal_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                NavigationService.Navigate(new LoginPage());
            }
            else
            {
                NavigationService.Navigate(new PersonalPage());
            }
        }
    }
}