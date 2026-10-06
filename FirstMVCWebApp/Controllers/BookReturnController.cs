using FirstMVCWebApp.Data;
using FirstMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace FirstMVCWebApp.Controllers
{
    public class BookReturnController : Controller
    {
        private readonly AppDbContext _context;

        public BookReturnController(AppDbContext context)
        {
            _context = context;
        }

        // 🔍 1. पेज लोड और सर्च करने का Get मेथड
        [HttpGet]
        public IActionResult Return()
        {
            // Dropdowns के लिए डेटा
            ViewBag.StudentList = _context.StudentDetails != null ? _context.StudentDetails
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Id} - {s.Name}" }).ToList()
                : new List<SelectListItem>();

            ViewBag.BookList = _context.Books != null ? _context.Books
                .Select(b => new SelectListItem { Value = b.BookId.ToString(), Text = $"{b.BookId} - {b.BookName}" }).ToList()
                : new List<SelectListItem>();

            
            ViewBag.AllReturns = (from br in _context.BookReturn
                                  join b in _context.Books on br.BookId equals b.BookId into bookGroup
                                  from b in bookGroup.DefaultIfEmpty()
                                  join s in _context.StudentDetails on br.StudentId equals s.Id into studentGroup
                                  from s in studentGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      ReturnId = br.ReturnId,
                                      BookName = b != null ? b.BookName : "Unknown Book",
                                      StudentName = s != null ? s.Name : "Unknown Student",
                                      ReturnDate = br.ReturnDate,
                                      FineAmount = br.FineAmount // 👈 यह लाइन मिसिंग थी, इसे जोड़ दिया है
                                  }).ToList();
          

            var model = new BookReturn { ReturnDate = DateTime.Today };
            return View(model);
        }



        // 💾 2. फॉर्म सबमिट (Save) करने का Post मेथड
        [HttpPost]
        public IActionResult SaveReturn(BookReturn model)
        {
            // 1. Find the active issue record for this specific student and book
            var activeIssue = _context.BookIssue
                .FirstOrDefault(i => i.BookId == model.BookId && i.StudentId == model.StudentId);

            if (activeIssue != null)
            {
                // 2. Define the expected due date (e.g., 7 days after the issue date)
                DateTime expectedDueDate = activeIssue.DueDate;
                DateTime actualReturnDate = model.ReturnDate;

                decimal calculatedFine = 0;

                // 3. If the return date is past the expected due date, calculate the fine
                if (actualReturnDate > expectedDueDate)
                {
                    int lateDays = (actualReturnDate - expectedDueDate).Days;
                    decimal finePerDay = 5; // Change this value to your preferred fine rate per day
                    calculatedFine = lateDays * finePerDay;
                }

                // 4. Assign the calculated value to the model right before saving
                model.FineAmount = calculatedFine;


                // 5. Your existing database saving logic
                _context.BookIssue.Remove(activeIssue);
            }
            _context.BookReturn.Add(model);

            // Update book stock logic (+1)
            var book = _context.Books.FirstOrDefault(b => b.BookId == model.BookId);
            if (book != null) { book.Quantity += 1; }

            
            _context.SaveChanges();

            return RedirectToAction("ReturnSuccess");
        }

        

        
        [HttpGet]
        [Route("BookReturn/ReturnSuccess")]
        public IActionResult ReturnSuccess()
        {
            return View();
        }


    }
}
