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
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();
            LoadCart();
        }

        private void LoadCart()
        {
            LvCart.ItemsSource = Core.Cart;
            decimal total = Core.Cart.Sum(x => x.TotalPrice);
            TbTotal.Text = $"Итого: {total:C}";
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            NavigationService.Navigate(new CheckoutPage());
        }
    }
}