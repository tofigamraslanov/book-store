using System.Collections.Generic;
using BulkyBook.Entities;

namespace BulkyBookWeb.ViewModels
{
    public class CategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}