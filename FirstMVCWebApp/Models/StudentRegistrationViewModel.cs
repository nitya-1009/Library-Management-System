using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FirstMVCWebApp.Models
{
    public class StudentRegistrationViewModel
    {

        //When you submit the form, Student carries the new data to the database, and when the page loads, Students brings back the entire list of existing records from the database to the screen. This is why both of them are created inside this single ViewModel."
        public StudentDetails? Student { get; set; }
        public List<StudentDetails>? Students { get; set; }


        // Add this property to hold your dropdown data
        public List<SelectListItem> CourseList { get; set; } = new List<SelectListItem>();
    }
}