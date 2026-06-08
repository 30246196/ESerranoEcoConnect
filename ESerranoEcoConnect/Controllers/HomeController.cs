using ESerranoEcoConnect.Models;
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

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us";

            return View();
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

        //public ActionResult Details(int id)
        //{
        //    var post = context.Posts
        //        .Include("Category")
        //        .Include("Staff")
        //        .FirstOrDefault(p => p.PostId == id);

        //    if (post == null)
        //        return HttpNotFound();

        //    // Load comments
        //    post.Comments = context.Comments
        //        .Include("Author")
        //        .Where(c => c.PostId == id)
        //        .OrderByDescending(c => c.CreatedAt)
        //        .ToList();

        //    return View(post);
        //}

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