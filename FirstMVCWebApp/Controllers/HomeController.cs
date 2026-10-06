using FirstMVCWebApp.Data;
using FirstMVCWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using static System.Reflection.Metadata.BlobBuilder;

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
        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            // 1. Fetching dashboard card counts
            ViewBag.StudentsListCount = _context.StudentDetails != null ? _context.StudentDetails.Count() : 0;
            ViewBag.BooksListCount = _context.Books != null ? _context.Books.Count() : 0;
            ViewBag.IssuesListCount = _context.BookIssue != null ? _context.BookIssue.Count() : 0;
            ViewBag.ReturnsListCount = _context.BookReturn != null ? _context.BookReturn.Count() : 0;

            // 2. Base list data fallbacks
            ViewBag.StudentsList = _context.StudentDetails != null ? _context.StudentDetails.ToList() : new List<StudentDetails>();
            ViewBag.booksList = _context.Books != null ? _context.Books.ToList() : new List<Book>();

            // 3. INNER JOIN for Book Issues (Grabs StudentName and BookName)
            if (_context.BookIssue != null && _context.Books != null && _context.StudentDetails != null)
            {
                ViewBag.BookIssueList = (from issue in _context.BookIssue
                                         join book in _context.Books on issue.BookId equals book.BookId
                                         join student in _context.StudentDetails on issue.StudentId equals student.Id
                                         select new
                                         {
                                             BookName = book.BookName,
                                             StudentName = student.Name,
                                             IssueDate = issue.IssueDate,
                                             IssueTime = issue.IssueTime
                                         }).ToList();
            }
            else
            {
                ViewBag.BookIssueList = new List<object>();
            }

            // 4. INNER JOIN for Book Returns (Grabs StudentName and BookName)
            if (_context.BookReturn != null && _context.Books != null && _context.StudentDetails != null)
            {
                ViewBag.BookReturnList = (from ret in _context.BookReturn
                                          join book in _context.Books on ret.BookId equals book.BookId
                                          join student in _context.StudentDetails on ret.StudentId equals student.Id
                                          select new
                                          {
                                              BookName = book.BookName,
                                              StudentName = student.Name,
                                              ReturnDate = ret.ReturnDate
                                          }).ToList();
            }
            else
            {
                ViewBag.BookReturnList = new List<object>();
            }

            return View();
        }



        [HttpGet]
        public IActionResult Analytics()
        {
            // 1. डेटाबेस से course wise स्टूडेंट्स की गिनती लाना
            var studentData = _context.StudentDetails
                .GroupBy(s => s.Course)
                .Select(g => new { CourseName = g.Key, Count = g.Count() })
                .ToList();

            var courseLabels = studentData.Select(d => d.CourseName ?? "Unknown").ToList();
            var courseCounts = studentData.Select(d => d.Count).ToList();

            if (courseLabels.Count == 0)
            {
                courseLabels = new List<string> { "BA", "BTECH", "BCOM" };
                courseCounts = new List<int> { 5, 4, 3 }; // टोटल 15 स्टूडेंट्स का ग्राफ बन जाएगा
            }

            var issuedBookIds = _context.BookIssue.Select(i => i.BookId).ToList();
            var returnedBookIds = _context.BookReturn.Select(i => i.BookId).ToList();

            int issuedBooks = _context.Books.Count(b => issuedBookIds.Contains(b.BookId) && !returnedBookIds.Contains(b.BookId));
            int availableBooks = _context.Books.Count(b => !issuedBookIds.Contains(b.BookId) && !returnedBookIds.Contains(b.BookId));

            List<int> bookStatus = new List<int>
            {
                availableBooks,
                issuedBookIds.Count,
                returnedBookIds.Count
            };
            // डेटा को व्यू पर भेजना
            ViewBag.CourseLabels = courseLabels;
            ViewBag.CourseCounts = courseCounts;
            ViewBag.BookStatusCounts = bookStatus;

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
        public IActionResult SearchStudents(string query)
        {
            var htmlBuilder = new System.Text.StringBuilder();

            var filteredStudents = _context.StudentDetails
                .Where(x => string.IsNullOrEmpty(query) ||
                            x.Name.Contains(query) ||
                            x.RollNo.Contains(query))
                .ToList();

            if (filteredStudents != null && filteredStudents.Any())
            {
                // Yahan humne dynamic use kiya hai taaki properties directly binary bind ho sakein
                foreach (dynamic s in filteredStudents)
                {
                    string status = (s.ActiveStatus != null && s.ActiveStatus.ToString().Trim().ToLower() == "active" ? "Active" : "Inactive");

                    htmlBuilder.Append($@"
                <tr>
                    <td>{s.Id}</td>
                    <td>{s.Name}</td>
                    <td>{s.Gender}</td>
                    <td>{s.Course}</td>
                    <td>{s.RollNo}</td>
                    <td>{s.Branch}</td>
                    <td>{s.FatherName}</td>
                    <td>{s.MotherName}</td>
                    <td>{s.Semester}</td>
                    <td>{s.Email}</td>
                    <td>{s.Phone}</td>
                    <td>{s.Address}</td>
                    <td>{s.City}</td>
                    <td>{s.Marks}</td>
                    <td>{s.CreatedOn}</td>
                    <td>{s.CreatedBy}</td>
                    <td>{status}</td>
                    <td>
                        <a href=""/Home/StudentBooks/{s.Id}"" class=""btn btn-info btn-sm"">View</a>
                    </td>
                    <td>
                        <a href=""/Home/Edit/{s.Id}"" class=""btn btn-primary btn-sm"">Edit</a>
                        <a href=""/Home/Delete/{s.Id}"" class=""btn btn-danger btn-sm"">Delete</a>
                    </td>
                </tr>");
                }
            }

            return Content(htmlBuilder.ToString(), "text/html");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var student = _context.StudentDetails.FirstOrDefault(x => x.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentRegistrationViewModel();
            model.Student = student;

            // Crucial: Yaha list fill karna mat bhuliyega line 168 ke liye
            model.Students = _context.StudentDetails.ToList();

            return View(model);
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

                    model.Student.ActiveStatus = true;
                    model.Student.CreatedOn = DateTime.Now; // यह आज की बिल्कुल सही तारीख और समय डाल देगा
                    model.Student.CreatedBy = "NITYA";      // जो भी आपका लॉगिन यूजर या डिफ़ॉल्ट नाम है


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

                return RedirectToAction("StudentRegistration", "Home");

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

    
      
    [HttpGet]
        [Authorize]
        public IActionResult StudentBooks(int id)
        {
            var student = _context.StudentDetails.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            ViewBag.StudentName = student.Name;
            ViewBag.RollNo = student.RollNo;

            // 🔍 BookIssues टेबल से इस छात्र की जारी की गई किताबें निकालें
            var issuedBooks = _context.BookIssue.Where(i => i.StudentId == id).ToList();

            ViewBag.BooksList = _context.Books.ToList();
            ViewBag.IssuedBooks = issuedBooks;

            return View();

        }

        

    }
}

