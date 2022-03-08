using System.Collections.Generic;
using BulkyBook.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> CoverTypeList { get; set; }
    }
}