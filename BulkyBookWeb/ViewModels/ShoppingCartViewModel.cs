using System.Collections.Generic;
using BulkyBook.Entities;

namespace BulkyBook.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}