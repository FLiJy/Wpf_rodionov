using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace pr13
{
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = Core.Context.Products.ToList();

            foreach (var product in products)
            {
                var border = new Border
                {
                    Width = 250,
                    Height = 350,
                    Margin = new Thickness(10),
                    Background = System.Windows.Media.Brushes.White,
                    BorderBrush = System.Windows.Media.Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5)
                };

                var stack = new StackPanel();

                // Картинка
                var img = new Image
                {
                    Height = 150,
                    Stretch = System.Windows.Media.Stretch.Uniform,
                    Margin = new Thickness(5)
                };

                try
                {
                    if (!string.IsNullOrEmpty(product.ImagePath) && File.Exists(product.ImagePath))
                        img.Source = new BitmapImage(new Uri(product.ImagePath));
                    else
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/noimage.jpg"));
                }
                catch { }

                // Название
                var title = new TextBlock
                {
                    Text = product.ProductName,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Margin = new Thickness(5),
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };

                // Цена
                var price = new TextBlock
                {
                    Text = $"{product.Price:C}",
                    FontSize = 18,
                    Foreground = System.Windows.Media.Brushes.Green,
                    Margin = new Thickness(5),
                    TextAlignment = TextAlignment.Center
                };

                // Кнопка добавить
                var btn = new Button
                {
                    Content = "В корзину",
                    Margin = new Thickness(10),
                    Padding = new Thickness(10),
                    Background = System.Windows.Media.Brushes.Orange,
                    Foreground = System.Windows.Media.Brushes.White,
                    Tag = product
                };
                btn.Click += AddToCart_Click;

                stack.Children.Add(img);
                stack.Children.Add(title);
                stack.Children.Add(price);
                stack.Children.Add(btn);
                border.Child = stack;

                ProductsWrapPanel.Children.Add(border);
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var product = btn.Tag as Products;

            var existing = Core.Cart.FirstOrDefault(x => x.Product.ProductID == product.ProductID);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                Core.Cart.Add(new CartItem { Product = product, Quantity = 1 });
            }

            MessageBox.Show($"{product.ProductName} добавлен в корзину!");

            // Обновляем счетчик в MainWindow
            if (Window.GetWindow(this) is MainWindow mw)
                mw.UpdateCartCount();
        }
    }
}