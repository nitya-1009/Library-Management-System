using System.ComponentModel.DataAnnotations;

namespace FirstMVCWebApp.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        public string CourseName { get; set; } = string.Empty;
    }
}