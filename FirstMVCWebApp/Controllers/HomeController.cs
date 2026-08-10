using FirstMVCWebApp.Data;
using FirstMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        // Function and model are same so we pass only model in return
        public IActionResult StudentRegistration(string globalSearch)
        {
            var model = new StudentRegistrationViewModel();

            // 1. Pehle saare active student records ko base query me load karo
            IQueryable<StudentDetails> query = _context.StudentDetails.Where(x => x.ActiveStatus == true);

            // 2. Global Search Logic - Yeh har ek column me data dhoondhega
            if (!string.IsNullOrEmpty(globalSearch))
            {
                // Taaki capital ya small letter likhne par bhi perfect search ho
                string search = globalSearch.Trim().ToLower();

                query = query.Where(s =>
                    (s.Name != null && s.Name.ToLower().Contains(search)) ||
                    (s.RollNo != null && s.RollNo.ToLower().Contains(search)) ||
                    (s.Gender != null && s.Gender.ToLower().Contains(search)) ||
                    (s.Course != null && s.Course.ToLower().Contains(search)) ||
                    (s.Branch != null && s.Branch.ToLower().Contains(search)) ||
                    (s.Semester != null && s.Semester.ToString().Contains(search)) ||
                    (s.FatherName != null && s.FatherName.ToLower().Contains(search)) ||
                    (s.MotherName != null && s.MotherName.ToLower().Contains(search)) ||
                    (s.Email != null && s.Email.ToLower().Contains(search)) ||
                    (s.Phone != null && s.Phone.ToLower().Contains(search)) ||
                    (s.Address != null && s.Address.ToLower().Contains(search)) ||
                    (s.City != null && s.City.ToLower().Contains(search)) ||
                    (s.Marks.ToString() == search) ||
                    (search == "active" && s.ActiveStatus == true) ||
                    (search == "inactive" && s.ActiveStatus == false)
 );


            }

            // 3. Filtered data ko list me convert karke model me daalein
            model.Students = query.ToList();
            return View(model);
        }
       
        [HttpGet]
        // Function and model are different so we pass model name in return 
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
        [Authorize]
        public IActionResult Save(StudentRegistrationViewModel model)
        {
            // Add this line to stop the validation error you just received:
            ModelState.Remove("Students");

            // Keep your existing remove lines:
            ModelState.Remove("Student.Id");
            ModelState.Remove("Student.CreatedBy");
            ModelState.Remove("Student.CreatedOn");
            ModelState.Remove("Student.Status");
            ModelState.Remove("Student.ActiveStatus");
            ModelState.Remove("Student.Action");

            if (ModelState.IsValid)
            {
                // If it's a new student record (Id is 0)
                if (model.Student.Id == 0)
                {
                    _context.StudentDetails.Add(model.Student);
                }
                // If it's an existing student record being updated
                else
                {
                    // 1. Attach the entity so Entity Framework starts tracking it
                    _context.StudentDetails.Attach(model.Student);

                    // 2. Explicitly tell EF which fields are allowed to change
                    _context.Entry(model.Student).Property(x => x.Name).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.Gender).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.Course).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.RollNo).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.Branch).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.Semester).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.FatherName).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.MotherName).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.Email).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.Phone).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.Address).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.City).IsModified = true;
                    _context.Entry(model.Student).Property(x => x.Marks).IsModified = true;

                    // By omitting CreatedBy, CreatedOn, and Status here, they are safely ignored and preserved!
                }

                // CRITICAL: You must save changes to the database
                _context.SaveChanges();

                // Redirect back to your main list view page
                return RedirectToAction("Index");
            }


         

            // 3. If validation FAILS, reload the dropdown lists and return the view with errors
            foreach (var item in ModelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    Console.WriteLine($"{item.Key} : {error.ErrorMessage}");
                }
            }

            // Repopulate your UI dropdown lists/tables before returning the view
            model.Students = _context.StudentDetails.ToList();
            model.CourseList = _context.StudentDetails
                .Select(c => c.Course)
                .Distinct()
                .Select(course => new SelectListItem
                {
                    Text = course,
                    Value = course
                }).ToList();

            return View("StudentRegistration", model);
        }

    }
}
