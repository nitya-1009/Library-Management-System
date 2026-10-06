using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstMVCWebApp.Models
{
    [Table("BookReturn")]
    public class BookReturn
    {
        [Key]
        public int ReturnId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int BookId { get; set; }

        public DateTime ReturnDate { get; set; } = DateTime.Now;

        public decimal FineAmount { get; set; } = 0;
    }
}
