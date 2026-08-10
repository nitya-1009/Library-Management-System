//server side validation
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstMVCWebApp.Models
{
    [Table("StudentDetails")]
    public class StudentDetails
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Course is Required")]
        public string? Course { get; set; }

        [Required(ErrorMessage = "RollNo is Required")]
        public string? RollNo { get; set; }
        public string? Branch { get; set; }

        [Required(ErrorMessage = "FatherName is Required")]
        public string? FatherName { get; set; }

        [Required(ErrorMessage = "MotherName is Required")]
        public string? MotherName { get; set; }

        [Required(ErrorMessage = "Semester is Required")]
        public int? Semester { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid character or format in Email.")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Phone is Required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "City is Required")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Marks is Required")]
        public int? Marks { get; set; }

        
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? ActiveStatus { get; set; }

        
    }
}