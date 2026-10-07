using FirstMVCWebApp.Data; // Apne actual Data folder ka namespace check kar lein
using FirstMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FirstMVCWebApp.Controllers
{
    public class BookIssueController : Controller
    {
        private readonly AppDbContext _context;

        public BookIssueController(AppDbContext context)
        
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Issue()
        {
            // Dropdowns के लिए डेटा (Students और Books)
            ViewBag.StudentList = _context.StudentDetails
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Id} - {s.Name}" }).ToList();

            ViewBag.BookList = _context.Books
                .Select(b => new SelectListItem { Value = b.BookId.ToString(), Text = $"{b.BookId} - {b.BookName}" }).ToList();

           
            ViewBag.AllBooks = (from bi in _context.BookIssue
                                join b in _context.Books on bi.BookId equals b.BookId into bookGroup
                                from b in bookGroup.DefaultIfEmpty()
                                join s in _context.StudentDetails on bi.StudentId equals s.Id into studentGroup
                                from s in studentGroup.DefaultIfEmpty()
                                select new
                                {
                                    IssueId = bi.IssueId, // 👈 इसे 'Id' से बदलकर 'IssueId' किया
                                    BookName = b != null ? b.BookName : "Unknown Book",
                                    StudentName = s != null ? s.Name : "Unknown Student",
                                    IssueDate = bi.IssueDate,
                                    IssueTime = bi.IssueTime,
                                    DueDate = bi.DueDate
                                }).ToList();

            var model = new BookIssue { IssueDate = DateTime.Today, DueDate = DateTime.Today.AddDays(7) };
            return View(model);
        }


        // 2. POST: Form Submit Hone Par Database Mein Save Karne Ke Liye
        [HttpPost]
        public IActionResult IssueBook(BookIssue issueObj)
        {
            if (issueObj != null && issueObj.BookId > 0 && issueObj.StudentId > 0)
            {
                // Current system ka samay TimeSpan mein set karne ke liye
                issueObj.IssueTime = DateTime.Now.TimeOfDay;
              
                _context.BookIssue.Add(issueObj);
                _context.SaveChanges();

                return RedirectToAction("IssueSuccess"); // Success page ya redirect list par
            }

            // Agar validation fail ho toh dropdowns reload karke wapas bhejein
            ViewBag.StudentList = _context.StudentDetails
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Id} - {s.Name}" })
                .ToList();

            ViewBag.BookList = _context.Books
                .Select(b => new SelectListItem { Value = b.BookId.ToString(), Text = $"{b.BookId} - {b.BookName}" })
                .ToList();

            return View("Issue", issueObj);
        }

        [HttpGet]
        public IActionResult IssueSuccess()
        {
            return View();
        }
    }
}
