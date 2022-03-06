using System.Collections.Generic;
using BulkyBook.Entities;

namespace BulkyBook.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}