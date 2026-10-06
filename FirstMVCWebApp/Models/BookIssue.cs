using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstMVCWebApp.Models
{
    [Table("BookIssue")] // Yeh batayega ki SSMS ki 'BookIssue' table se connect hona hai
    public class BookIssue
    {
        [Key]
        public int IssueId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required]
        public TimeSpan IssueTime { get; set; } // SSMS ke TIME data type ke liye TimeSpan sabse sahi hai

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

       

    }
}
