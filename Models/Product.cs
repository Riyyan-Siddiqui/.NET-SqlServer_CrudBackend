using System.ComponentModel.DataAnnotations;

namespace CrudBackend_1_.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; } = 0.00m;
        public string Color { get; set; } = null!;

    }
}
