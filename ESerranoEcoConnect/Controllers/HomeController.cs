using ESerranoEcoConnect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity; // Add this using directive
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESerranoEcoConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly EcoConnectDbContext context = new EcoConnectDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "EcoConnect Community";

            return View();
        }

        // Stage 11: Add a Contact Form

        //GET: Contact
        public ActionResult Contact()
        {
            return View();
        }

        // POST: Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ModelState.AddModelError("Message", "Message cannot be empty.");
                return View();
            }

            var contact = new ContactForm
            {
                Message = message,
                SentAt = DateTime.Now,
                UserId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null
            };

            context.ContactForms.Add(contact);
            context.SaveChanges();

            TempData["SuccessMessage"] = "Your message has been sent successfully.";

            return RedirectToAction("Contact");
        }

        // ADMIN INBOX
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult ContactInbox()
        {
            var messages = context.ContactForms
                .OrderByDescending(c => c.SentAt)
                .ToList();

            return View(messages);
        }
        
    



    public ActionResult GetBlogs()
        {
            // Get all posts, include category and staff author
            var posts = context.Posts        
                .Include("Category") // Use string overload for Include
                .Include("Staff")    // Use string overload for Include
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            // Send categories to the view for the category filter
            ViewBag.Categories = context.Categories.ToList();

            return View("Getblogs",posts);
        }
      

        [HttpPost]
        public ActionResult GetBlogs(string SearchString)
        {
            // If no search text, return all posts
            if (string.IsNullOrWhiteSpace(SearchString))
            {
                return RedirectToAction("GetBlogs");
            }

            // Filter posts by category name
            var posts = context.Posts
                .Include("Category")
                .Include("Staff")
                .Where(p => p.Category.CategoryName.Equals(SearchString.Trim()))
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            // Send categories again for the view
            ViewBag.Categories = context.Categories.ToList();

            return View("GetBlogs",posts);
        }        

    }
}