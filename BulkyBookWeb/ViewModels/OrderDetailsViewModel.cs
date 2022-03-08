using System.Collections.Generic;
using BulkyBook.Entities;

namespace BulkyBookWeb.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}