using FirstMVCWebApp.Data;
using FirstMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FirstMVCWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult StudentRegistration()
        {
            var model = new StudentRegistrationViewModel();

            // Fill the table collection so line 168 does not break
            model.Students = _context.StudentDetails
                .Where(x => x.ActiveStatus == true)
                .ToList();

            // Fill the dropdown list
            model.CourseList = _context.Courses
                .Select(c => c.Course.ToUpper().Trim())
                .Distinct()
                .Select(course => new SelectListItem { Text = course, Value = course })
                .ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _context.StudentDetails.FirstOrDefault(x => x.Id == id);
            if (student == null) return NotFound();

            var model = new StudentRegistrationViewModel();
            model.Student = student;

            // Crucial: You MUST also fill the table list here for line 168 to work!
            model.Students = _context.StudentDetails.ToList();

            // Fill the dropdown list
            model.CourseList = _context.Courses
                .Select(c => c.Course.ToUpper().Trim())
                .Distinct()
                .Select(course => new SelectListItem { Text = course, Value = course })
                .ToList();

            return View("StudentRegistration", model);
        }

        public IActionResult Delete(int id)
        {
            var student = _context.StudentDetails.FirstOrDefault(x => x.Id == id);

            if (student != null)
            {
                student.ActiveStatus = false;
                student.UpdatedOn = DateTime.Now;
                student.UpdatedBy = "Admin";

                _context.SaveChanges();
            }

            return RedirectToAction("StudentRegistration");
        }

        [HttpPost]
        public IActionResult Save(StudentRegistrationViewModel model)
        {
            // 1. Clear validation tracking for unsubmitted fields
            ModelState.Remove("Student.Id");
            ModelState.Remove("Student.CreatedBy");
            ModelState.Remove("Student.CreatedOn");
            ModelState.Remove("Student.Status");
            ModelState.Remove("Student.ActiveStatus");
            ModelState.Remove("Student.Action");

            // 2. UPDATE EXISTING RECORD
            if (model.Student != null && model.Student.Id > 0)
            {
                var dbStudent = _context.StudentDetails.FirstOrDefault(x => x.Id == model.Student.Id);
                if (dbStudent != null)
                {
                    dbStudent.Name = model.Student.Name;
                    dbStudent.RollNo = model.Student.RollNo;
                    dbStudent.Gender = model.Student.Gender;
                    dbStudent.Course = model.Student.Course;
                    dbStudent.Branch = model.Student.Branch;
                    dbStudent.Semester = model.Student.Semester;
                    dbStudent.FatherName = model.Student.FatherName;
                    dbStudent.MotherName = model.Student.MotherName;
                    dbStudent.Email = model.Student.Email;
                    dbStudent.Phone = model.Student.Phone;
                    dbStudent.City = model.Student.City;
                    dbStudent.Marks = model.Student.Marks;
                    dbStudent.Address = model.Student.Address;
                    dbStudent.ActiveStatus = model.Student.ActiveStatus;

                    _context.SaveChanges();
                    return RedirectToAction("StudentRegistration");
                }
            }
            // 3. CREATE NEW RECORD (Bypassed ModelState to force insertion)
            else if (model.Student != null)
            {
                
                
                model.Student.CreatedBy = User.Identity?.Name ?? "System Admin";

                model.Student.CreatedOn = DateTime.Now;
                model.Student.ActiveStatus = true; 
                _context.StudentDetails.Add(model.Student);
                _context.SaveChanges();

                return RedirectToAction("StudentRegistration");
            }

            // 4. Fallback: Repopulate drop-downs if something goes wrong
            model.Students = _context.StudentDetails?.ToList() ?? new List<StudentDetails>();
            model.CourseList = _context.Courses
                .Select(c => c.Course != null ? c.Course.ToUpper().Trim() : "")
                .Distinct()
                .Select(course => new SelectListItem { Text = course, Value = course })
                .ToList();

            return View("StudentRegistration", model);
        }
    }
}
