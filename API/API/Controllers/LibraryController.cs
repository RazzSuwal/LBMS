﻿using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly IBookService _bookService;
        public LibraryController(Context context, JwtService jwtService, IBookService bookService)
        {
            Context = context;
            //EmailService = emailService;
            JwtService = jwtService;
            _bookService = bookService;
        }

        public Context Context { get; }
        //public EmailService EmailService { get; }
        public JwtService JwtService { get; }

        [HttpPost("Register")]
        public ActionResult Register(User user)
        {
            user.AccountStatus = AccountStatus.UNAPROOVED;
            user.UserType = UserType.STUDENT;
            user.CreatedOn = DateTime.Now;

            Context.Users.Add(user);
            Context.SaveChanges();

            const string subject = "Account Created";
            var body = $"""
                <html>
                    <body>
                        <h1>Hello, {user.FirstName} {user.LastName}</h1>
                        <h2>
                            Your account has been created and we have sent approval request to admin. 
                            Once the request is approved by admin you will receive email, and you will be
                            able to login in to your account.
                        </h2>
                        <h3>Thanks</h3>
                    </body>
                </html>
            """;

            //EmailService.SendEmail(user.Email, subject, body);

            return Ok(@"Thank you for registering. 
                        Your account has been sent for aprooval. 
                        Once it is aprooved, you will get an email.");
        }

        [HttpGet("Login")]
        public ActionResult Login(string email, string password)
        {
            if (Context.Users.Any(u => u.Email.Equals(email) && u.Password.Equals(password)))
            {
                var user = Context.Users.Single(user => user.Email.Equals(email) && user.Password.Equals(password));

                if (user.AccountStatus == AccountStatus.UNAPROOVED)
                {
                    return Ok("unapproved");
                }

                if (user.AccountStatus == AccountStatus.BLOCKED)
                {
                    return Ok("blocked");
                }

                return Ok(JwtService.GenerateToken(user));
            }
            return Ok("not found");
        }

        [Authorize]
        [HttpGet("GetBooks")]
        public ActionResult GetBooks()
        {
            if (Context.Books.Any())
            {
                return Ok(Context.Books.Include(b => b.BookCategory).ToList());
            }
            return NotFound();
        }
        [Authorize]
        [HttpGet("GetBooksById")]
        public async Task<IActionResult> GetBooksById(int id)
        {
            try
            {
                var books = await _bookService.GetBookByIdAsync(id);

                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving books: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("OrderBook")]
        public ActionResult OrderBook(int userId, int bookId)
        {
            var canOrder = Context.Orders.Count(o => o.UserId == userId && !o.Returned) < 3;

            if (canOrder)
            {
                Context.Orders.Add(new()
                {
                    UserId = userId,
                    BookId = bookId,
                    OrderDate = DateTime.Now,
                    ReturnDate = null,
                    Returned = false,
                    FinePaid = 0
                });

                var book = Context.Books.Find(bookId);
                if (book is not null)
                {
                    book.Ordered = true;
                }


                Context.SaveChanges();
                return Ok("ordered");
            }

            return Ok("cannot order");
        }

        [Authorize]
        [HttpGet("GetOrdersOFUser")]
        public ActionResult GetOrdersOFUser(int userId)
        {
            var orders = Context.Orders
                .Include(o => o.Book)
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
                .ToList();
            if (orders.Any())
            {
                return Ok(orders);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("AddCategory")]
        [Authorize]
        public ActionResult AddCategory(BookCategory bookCategory)
        {
            var exists = Context.BookCategories.Any(bc => bc.Category == bookCategory.Category && bc.SubCategory == bookCategory.SubCategory);
            if (exists)
            {
                return Ok("cannot insert");
            }
            else
            {
                Context.BookCategories.Add(new()
                {
                    Category = bookCategory.Category,
                    SubCategory = bookCategory.SubCategory
                });
                Context.SaveChanges();
                return Ok("INSERTED");
            }
        }

        [Authorize]
        [HttpGet("GetCategories")]
        public ActionResult GetCategories()
        {
            var categories = Context.BookCategories.ToList();
            if (categories.Any())
            {
                return Ok(categories);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost("AddBook")]
        public ActionResult AddBook(Book book)
        {
            book.BookCategory = null;
            Context.Books.Add(book);
            Context.SaveChanges();
            return Ok("inserted");
        }

        [Authorize]
        [HttpDelete("DeleteBook")]
        public ActionResult DeleteBook(int id)
        {
            var exists = Context.Books.Any(b => b.Id == id);
            if (exists)
            {
                var book = Context.Books.Find(id);
                Context.Books.Remove(book!);
                Context.SaveChanges();
                return Ok("deleted");
            }
            return NotFound();
        }

        [HttpGet("ReturnBook")]
        public ActionResult ReturnBook(int userId, int bookId, int fine)
        {
            var order = Context.Orders.SingleOrDefault(o => o.UserId == userId && o.BookId == bookId);
            if (order is not null)
            {
                order.Returned = true;
                order.ReturnDate = DateTime.Now;
                order.FinePaid = fine;

                var book = Context.Books.Single(b => b.Id == order.BookId);
                book.Ordered = false;

                Context.SaveChanges();

                return Ok("returned");
            }
            return Ok("not returned");
        }

        [Authorize]
        [HttpGet("GetUsers")]
        public ActionResult GetUsers()
        {
            return Ok(Context.Users.ToList());
        }

        [Authorize]
        [HttpGet("ApproveRequest")]
        public ActionResult ApproveRequest(int userId)
        {
            var user = Context.Users.Find(userId);

            if (user is not null)
            {
                if (user.AccountStatus == AccountStatus.UNAPROOVED)
                {
                    user.AccountStatus = AccountStatus.ACTIVE;
                    Context.SaveChanges();

                    //EmailService.SendEmail(user.Email, "Account Approved", $"""
                    //    <html>
                    //        <body>
                    //            <h2>Hi, {user.FirstName} {user.LastName}</h2>
                    //            <h3>You Account has been approved by admin.</h3>
                    //            <h3>Now you can login to your account.</h3>
                    //        </body>
                    //    </html>
                    //""");

                    return Ok("approved");
                }
            }

            return Ok("not approved");
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public ActionResult GetOrders()
        {
            var orders = Context.Orders.Include(o => o.User).Include(o => o.Book).ToList();
            if (orders.Any())
            {
                return Ok(orders);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpGet("SendEmailForPendingReturns")]
        public ActionResult SendEmailForPendingReturns()
        {
            var orders = Context.Orders
                            .Include(o => o.Book)
                            .Include(o => o.User)
                            .Where(o => !o.Returned)
                            .ToList();

            var emailsWithFine = orders.Where(o => DateTime.Now > o.OrderDate.AddDays(10)).ToList();
            emailsWithFine.ForEach(x => x.FinePaid = (DateTime.Now - x.OrderDate.AddDays(10)).Days * 50);

            var firstFineEmails = emailsWithFine.Where(x => x.FinePaid == 50).ToList();
            firstFineEmails.ForEach(x =>
            {
                var body = $"""
                <html>
                    <body>
                        <h2>Hi, {x.User?.FirstName} {x.User?.LastName}</h2>
                        <h4>Yesterday was your last day to return Book: "{x.Book?.Title}".</h4>
                        <h4>From today, every day a fine of 50Rs will be added for this book.</h4>
                        <h4>Please return it as soon as possible.</h4>
                        <h4>If your fine exceeds 500Rs, your account will be blocked.</h4>
                        <h4>Thanks</h4>
                    </body>
                </html>
                """;

                //EmailService.SendEmail(x.User!.Email, "Return Overdue", body);
            });

            //var regularFineEmails = emailsWithFine.Where(x => x.FinePaid > 50 && x.FinePaid <= 500).ToList();
            //regularFineEmails.ForEach(x =>
            //{
            //    var regularFineEmailsBody = $"""
            //    <html>
            //        <body>
            //            <h2>Hi, {x.User?.FirstName} {x.User?.LastName}</h2>
            //            <h4>You have {x.FinePaid}Rs fine on Book: "{x.Book?.Title}"</h4>
            //            <h4>Pleae pay it as soon as possible.</h4>
            //            <h4>Thanks</h4>
            //        </body>
            //    </html>
            //    """;

            //    EmailService.SendEmail(x.User?.Email!, "Fine To Pay", regularFineEmailsBody);
            //});

            //var overdueFineEmails = emailsWithFine.Where(x => x.FinePaid > 500).ToList();
            //overdueFineEmails.ForEach(x =>
            //{
            //    var overdueFineEmailsBody = $"""
            //    <html>
            //        <body>
            //            <h2>Hi, {x.User?.FirstName} {x.User?.LastName}</h2>
            //            <h4>You have {x.FinePaid}Rs fine on Book: "{x.Book?.Title}"</h4>
            //            <h4>Your account is BLOCKED.</h4>
            //            <h4>Pleae pay it as soon as possible to UNBLOCK your account.</h4>
            //            <h4>Thanks</h4>
            //        </body>
            //    </html>
            //    """;

            //    EmailService.SendEmail(x.User?.Email!, "Fine Overdue", overdueFineEmailsBody);
            //});

            return Ok("sent");
        }

        [Authorize]
        [HttpGet("BlockFineOverdueUsers")]
        public ActionResult BlockFineOverdueUsers()
        {
            var orders = Context.Orders
                            .Include(o => o.Book)
                            .Include(o => o.User)
                            .Where(o => !o.Returned)
                            .ToList();

            var emailsWithFine = orders.Where(o => DateTime.Now > o.OrderDate.AddDays(10)).ToList();
            emailsWithFine.ForEach(x => x.FinePaid = (DateTime.Now - x.OrderDate.AddDays(10)).Days * 50);

            var users = emailsWithFine.Where(x => x.FinePaid > 500).Select(x => x.User!).Distinct().ToList();

            if (users is not null && users.Any())
            {
                foreach (var user in users)
                {
                    user.AccountStatus = AccountStatus.BLOCKED;
                }
                Context.SaveChanges();

                return Ok("blocked");
            }
            else
            {
                return Ok("not blocked");
            }
        }

        [Authorize]
        [HttpGet("Unblock")]
        public ActionResult Unblock(int userId)
        {
            var user = Context.Users.Find(userId);
            if (user is not null)
            {
                user.AccountStatus = AccountStatus.ACTIVE;
                Context.SaveChanges();
                return Ok("unblocked");
            }

            return Ok("not unblocked");
        }

        [Authorize]
        [HttpPost("AddCustomer")]
        public ActionResult AddCustomer(Customer customer)
        {
            Context.Customers.Add(customer);
            Context.SaveChanges();
            return Ok("inserted");
        }
        [Authorize]
        [HttpPost("AddProduct")]
        public ActionResult AddProduct(Product Product)
        {
            Context.Products.Add(Product);
            Context.SaveChanges();
            return Ok("inserted");
        }

        [Authorize]
        [HttpPost("AddBilling")]
        public ActionResult AddBilling(Billing Billing)
        {
            Context.Billings.Add(Billing);
            Context.SaveChanges();
            return Ok("inserted");
        }



        [HttpPost("CreateBilling")]
        public async Task<IActionResult> CreateBillingWithProduct([FromBody] BillingViewModel viewModel)
        {
            if (viewModel == null || string.IsNullOrEmpty(viewModel.CustomerName) || viewModel.Products == null || viewModel.Products.Count == 0)
            {
                return BadRequest("Invalid data.");
            }

            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Check if customer already exists or create new
                    Customer customer;
                    if (viewModel.CustomerId > 0)
                    {
                        customer = await Context.Customers.FindAsync(viewModel.CustomerId);
                        if (customer == null)
                        {
                            return NotFound("Customer not found.");
                        }
                    }
                    else
                    {
                        // Create new customer
                        customer = new Customer
                        {
                            DocumentNo = viewModel.DocumentNo,
                            CustomerName = viewModel.CustomerName,
                            MobileNumber = viewModel.MobileNumber
                        };

                        Context.Customers.Add(customer);
                        await Context.SaveChangesAsync();
                    }

                    // Create new billing record associated with the customer
                    var billing = new Billing
                    {
                        Discount = viewModel.Discount,
                        GrandTotal = viewModel.GrandTotal,
                        CustomerId = customer.CustomerId // Associate with customer
                    };

                    Context.Billings.Add(billing);
                    await Context.SaveChangesAsync();

                    // Create new product records associated with the billing
                    foreach (var productModel in viewModel.Products)
                    {
                        var product = new Product
                        {
                            ProductName = productModel.ProductName,
                            Quantity = productModel.Quantity,
                            Rate = productModel.Rate,
                            Amount = productModel.Amount,
                            CustomerId = customer.CustomerId // Associate with customer
                        };

                        Context.Products.Add(product);
                    }

                    await Context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    // Return the created entities' IDs
                    return Ok(new { CustomerId = customer.CustomerId, BillingId = billing.BillingId });
                }
                catch (Exception ex)
                {
                    // Rollback transaction on error
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        [HttpGet("GetBooksWithPagination")]
        public async Task<IActionResult> GetBooksWithPagination(string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var books = await _bookService.GetBooksAsync(searchTerm, pageNumber, pageSize);

                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving books: {ex.Message}");
            }
        }

        [HttpPut("EditBook")]
        [Authorize]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
        {
            if (id != updatedBook.Id)
            {
                return BadRequest("The ID in the route parameter must match the ID in the request body.");
            }

            try
            {
                var result = await _bookService.UpdateBookByIdAsync(updatedBook);

                if (result == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the book: {ex.Message}");
            }
        }
    }
}