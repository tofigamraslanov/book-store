using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
    }
}