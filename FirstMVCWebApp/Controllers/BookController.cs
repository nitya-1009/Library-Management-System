using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using FirstMVCWebApp.Models;
using FirstMVCWebApp.Data;

namespace FirstMVCWebApp.Controllers
{
    [Authorize] 
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        // कन्सट्रक्टर के ज़रिए डेटाबेस को कंट्रोलर में लाना (Dependency Injection)
        public BookController(AppDbContext context)
        {
            _context = context;
        }

        // 1. किताबों की लिस्ट दिखाने के लिए (GET Method)

        // 1. यह आपका मुख्य पेज लोड करेगा (डिफ़ॉल्ट रूप से)
        public IActionResult Books()
        {
            var books = _context.Books.ToList();
            var issuedBookIds = _context.BookIssue.Select(i => i.BookId).ToList();
            var returnedBookIds = _context.BookReturn.Select(r => r.BookId).ToList();

            ViewBag.AllBooks = books.Select(b => new {
                b.BookId,
                b.BookName,
                b.Author,
                b.ISBN,
                b.Quantity,
                Status = returnedBookIds.Contains(b.BookId) ? "Returned" : 
                          (issuedBookIds.Contains(b.BookId) ? "Issued" : "Available")
            }).ToList<dynamic>();

            return View("Books");
        }

        // 2. नया मेथड: AJAX रिक्वेस्ट आने पर केवल टेबल की रोज़ (Rows) छानकर भेजेगा
        [HttpGet]
        public IActionResult SearchBooks(string query)
        {
            var booksQuery = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                query = query.Trim().ToLower();
                booksQuery = booksQuery.Where(b =>
                    b.BookName.ToLower().Contains(query) ||
                    b.Author.ToLower().Contains(query) ||
                    b.BookId.ToString().Contains(query)
                );
            }

            var issuedBookIds = _context.BookIssue.Select(i => i.BookId).ToList();
            var returnedBookIds = _context.BookReturn.Select(r => r.BookId).ToList();

            var filteredBooks = booksQuery.ToList();

            // Directly building the clean plain-text HTML string container to completely bypass view path errors
            System.Text.StringBuilder htmlBuilder = new System.Text.StringBuilder();

            if (filteredBooks.Any())
            {
                foreach (var b in filteredBooks)
                {
                    string status = issuedBookIds.Contains(b.BookId) ? "Issued" :
                                    (returnedBookIds.Contains(b.BookId) ? "Returned" : "Available");

                    string statusColor = status == "Issued" ? "#d69e2e" :
                                         (status == "Returned" ? "#3182ce" : "#38a169");

                    htmlBuilder.Append("<tr>");
                    htmlBuilder.Append($"<td>{b.BookId}</td>");
                    htmlBuilder.Append($"<td>{b.BookName}</td>");
                    htmlBuilder.Append($"<td>{b.Author}</td>");
                    htmlBuilder.Append($"<td>{b.ISBN}</td>");
                    htmlBuilder.Append($"<td>{b.Quantity}</td>");
                    htmlBuilder.Append($"<td><span class='fw-bold' style='color: {statusColor}; font-size: 0.9rem; letter-spacing: 0.5px;'>{status}</span></td>");
                    htmlBuilder.Append("<td>");
                    htmlBuilder.Append("<button class='btn btn-sm btn-warning text-dark px-2 fw-semibold me-1'>Edit</button>");
                    htmlBuilder.Append("<button class='btn btn-sm btn-danger px-2 fw-semibold'>Delete</button>");
                    htmlBuilder.Append("</td>");
                    htmlBuilder.Append("</tr>");
                }
            }
            else
            {
                htmlBuilder.Append("<tr><td colspan='7' class='text-center py-4 text-muted'>No records matching your search were found.</td></tr>");
            }

            return Content(htmlBuilder.ToString(), "text/html");
        }




        // 2. फॉर्म सबमिट होने पर डेटा सेव करने के लिए (POST Method)
        [HttpPost]
        public IActionResult SaveBook(Book bookObj)
        {
            // Strict validation hatakar sirf zaroori fields check kar rahe hain
            if (bookObj != null && !string.IsNullOrEmpty(bookObj.BookName))
            {
                if (bookObj.BookId == 0)
                {
                   

                    // Nayi book add karne ke liye
                    _context.Books.Add(bookObj);
                }

                else
                {
                    // Puraani book update karne ke liye
                    _context.Books.Update(bookObj);
                }

                _context.SaveChanges(); // Database mein save karein

                // Save karne ke baad page ko refresh (redirect) karein taaki list update ho jaye
                return RedirectToAction("Books");
            }

            // Agar data bilkul khali hai tabhi wapas view bhejenge
            ViewBag.AllBooks = _context.Books.ToList();
            return View("Books", bookObj);
        }



        // EDIT: Same Books page par existing book ko load karna
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);

            if (book == null)
            {
                return NotFound();
            }

            ViewBag.AllBooks = _context.Books.ToList();

            return View("Books", book);
        }


        // 2. EDIT (POST): Form submit hone par data update karne ke liye
        [HttpPost]
        [ValidateAntiForgeryToken] // Security ke liye zaroori hai
        public IActionResult Edit(Book bookObj)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Update(bookObj);
                _context.SaveChanges();

                // Data save hone ke baad wapas list wale page (Index) par bhejein
                return RedirectToAction(nameof(Index));
            }

            return View(bookObj);
        }

        // 3. DELETE (POST): Jo naya form button humne banaya tha, uske liye safe code
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF attack se bachane ke liye
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(id);

            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
          // Delete hone ke baad wapas list wale page (Index) par bhejein
            return RedirectToAction(nameof(Books));
        }
    }
}