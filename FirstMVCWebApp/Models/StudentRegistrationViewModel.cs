using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FirstMVCWebApp.Models
{
    public class StudentRegistrationViewModel
    {
        public StudentDetails Student { get; set; }
        public List<StudentDetails> Students { get; set; }

        // Add this property to hold your dropdown data
        public List<SelectListItem> CourseList { get; set; } = new List<SelectListItem>();
    }
}