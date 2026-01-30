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

namespace pr13
{
    public partial class CheckoutPage : Page
    {
        public CheckoutPage()
        {
            InitializeComponent();
            LvOrderItems.ItemsSource = Core.Cart;
            decimal total = Core.Cart.Sum(x => x.TotalPrice);
            TbFinalTotal.Text = $"К оплате: {total:C}";
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(TxtFIO.Text) ||
                string.IsNullOrWhiteSpace(TxtEmail.Text) ||
                string.IsNullOrWhiteSpace(TxtAddress.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            try
            {
                // Создаем заказ
                var order = new Orders
                {
                    FIO = TxtFIO.Text,
                    Email = TxtEmail.Text,
                    Address = TxtAddress.Text,
                    OrderDate = DateTime.Now,
                    TotalPrice = Core.Cart.Sum(x => x.TotalPrice)
                };

                Core.Context.Orders.Add(order);
                Core.Context.SaveChanges(); // Получаем OrderID

                // Добавляем элементы заказа
                foreach (var cartItem in Core.Cart)
                {
                    var orderItem = new OrderItems
                    {
                        OrderID = order.OrderID,
                        ProductID = cartItem.Product.ProductID,
                        Quantity = cartItem.Quantity,
                        PriceAtMoment = cartItem.Product.Price
                    };
                    Core.Context.OrderItems.Add(orderItem);
                }

                Core.Context.SaveChanges();

                MessageBox.Show("Заказ успешно оформлен!");

                // Очищаем корзину
                Core.Cart.Clear();

                // Обновляем счетчик и возвращаемся к товарам
                if (Window.GetWindow(this) is MainWindow mw)
                {
                    mw.UpdateCartCount();
                    mw.MainFrame.Navigate(new ProductsPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}