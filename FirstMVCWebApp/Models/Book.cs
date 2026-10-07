using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstMVCWebApp.Models
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Book name is required")]
        public string? BookName { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        public string? Author { get; set; }

        public string? ISBN { get; set; }

       
        [Required]
        public int Quantity { get; set; }

        
    }
}
