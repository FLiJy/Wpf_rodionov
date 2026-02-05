using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr13
{
    public class Core
    {
        public static PR13_Rodionov Context = new PR13_Rodionov();

        // Список для хранения корзины в памяти (не в БД)
        public static System.Collections.Generic.List<CartItem> Cart = new System.Collections.Generic.List<CartItem>();
    }

    // Класс для элемента корзины
    public class CartItem
    {
        public Products Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Product.Price * Quantity;
    }
}