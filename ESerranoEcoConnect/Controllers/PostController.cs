using ESerranoEcoConnect.Models;// added for the db context and the Post model
using ESerranoEcoConnect.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace ESerranoEcoConnect.Controllers
{
    public class PostController : Controller
    {
        // This controller will handle all the CRUD operations for the Post model

        // get the db context
        private EcoConnectDbContext db = new EcoConnectDbContext();

        // GET: Post
        public ActionResult Index()
        {
            // get all the posts from the database and pass them to the view
            var posts= db.Posts
                .Include(p => p.Category)
                .Include(p => p.Staff)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(posts);
        }

        //POST: Post/details/5
        public ActionResult Details(int id)
        {
            var post = db.Posts
                .Include(p => p.Category)
                .Include(p => p.Staff)
                .Include(p=> p.Comments.Select(c=>c.Author))
                .FirstOrDefault(p=> p.PostId == id);

            if (post == null)
                return HttpNotFound();

            var vm = new PostDetailsViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                CategoryName = post.Category.CategoryName,
                AuthorName = post.Staff.FirstName + " " + post.Staff.LastName,
                Comments = post.Comments.OrderByDescending(c => c.CreatedAt).ToList(),
                NewCommentContent = "" //otherwise in the content of comment writes the content of the post
                
            };

            ModelState.Clear();// razor without values
            return View(vm);

        }

        // GET: Post/Create
        [Authorize(Roles = "Staff,Admin")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Admin")]
        public ActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedAt = System.DateTime.Now;
                post.StaffId = User.Identity.GetUserId();

                db.Posts.Add(post);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }
    }
}