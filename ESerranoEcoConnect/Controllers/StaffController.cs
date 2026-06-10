using ESerranoEcoConnect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ESerranoEcoConnect.Controllers
{
    
    public class StaffController : Controller
    {
        private EcoConnectDbContext db = new EcoConnectDbContext();

        // GET:  Posts for the logged-in staff member
        [Authorize(Roles = "Staff")]
        public ActionResult Index()
        {
            // select the posts for the logged-in staff member
            string staffId = User.Identity.GetUserId();

            // Get all posts for the logged-in staff member, include category
            var posts = db.Posts
                .Include("Category")
                .Where(p => p.StaffId == staffId) // Filter posts by the logged-in staff member
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(posts.ToList());
        }

        // GET: Staff/Details/5
        public ActionResult Details(int? id) // create a nullable int parameter for the post ID
        {
            // Stage 6: Task a logged staff can see their own post Details page
            if (id == null)
            { 
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            //find a post in the Posts table with the given id, include the category and staff author
            Post post = db.Posts
                .Include("Category")
                .Include("Staff")
                .FirstOrDefault(p=>p.PostId==id);

            //if post does not exist, return 404 error
            if (post == null)
            {
                return HttpNotFound();
            }

            // otherwise send the post to the Details view
            // and display the post details, including the category name and staff author name

            return View(post);
        }

        // GET: Staff/Create
        public ActionResult Create()
            {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }


        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Content,CategoryId,IsAnnouncement,IsEventUpdate")] Post post)
        {
            // Assign StaffId before validation to ensure it is included in the ModelState           
            post.StaffId = User.Identity.GetUserId();

            //remove  StaffId from validation since it is not included in the form and is assigned automatically
            //ModelState["StaffId"].Errors.Clear();
            ModelState.Remove("StaffId");
            if (ModelState.IsValid)
            {
               // post.StaffId = User.Identity.GetUserId();
                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;

                db.Posts.Add(post);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }


        // GET: Staff/Edit/5
        public ActionResult Edit(int? id)// create a nullable int parameter for the post ID
        {
            // implemented in Stage 6: Task b logged staff can see their own post Edit
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            // find a post in the Posts table with the given id, include the category and staff author
            Post post = db.Posts.Find(id);

            // if post does not exist, return 404 error
            if (post == null)
            {
                return HttpNotFound();
            }

            //get alist of all categories from the Categories table and send it to the view using ViewBag
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);

            return View(post);
        }

        // POST: Staff/Edit/5
        // Stage 6: Task c logged staff can edit their own post Edit form and submit the changes to update the post in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,Title,Content,CategoryId,IsAnnouncement,IsEventUpdate,CreatedAt,StaffId")] Post post)
        {
            if (ModelState.IsValid)
            {
                var existingPost = db.Posts.Find(post.PostId);
                if (existingPost == null)
                    return HttpNotFound();

                // Editable fields
                existingPost.Title = post.Title;
                existingPost.Content = post.Content;
                existingPost.CategoryId = post.CategoryId;
                existingPost.IsAnnouncement = post.IsAnnouncement;
                existingPost.IsEventUpdate = post.IsEventUpdate;

                // Preserve original CreatedAt
                existingPost.CreatedAt = post.CreatedAt;

                // Auto-update timestamp
                existingPost.UpdatedAt = DateTime.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", post.CategoryId);
            return View(post);
        }


        // GET: Staff/Delete/5
        public ActionResult Delete(int? id)
        {
            // if id is null, return bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            //first find the post in the Posts table with the given id, include the category and staff author
            Post post = db.Posts.Find(id);

            //next find a post in the Posts table with the given id, include the category and staff author
            var category = db.Categories.Find(post.CategoryId);

            //assign the category name to the ViewBag to display it in the view
            post.Category = category;

            // if the post does not exist, return 404 error
            if (post == null)
            {
                return HttpNotFound();
            }

            //otherwise send the post to the Delete view and display the post details, including the category name and staff author name, and ask the user to confirm the deletion
            return View(post);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("Delete")]// Stage 6: Task d logged staff can delete their own post Delete form and submit the deletion to remove the post from the database
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //find post by id in Posts table
            Post post = db.Posts.Find(id);

            //GET ALL THE COMMENTS FOR THIS POST AND REMOVE THEM FROM THE DATABASE           
            List<Comment> commentsFromPost = db.Comments.Include(c => c.Post).Where(c => c.PostId == id).ToList();
            db.Comments.RemoveRange(commentsFromPost);
            db.SaveChanges();

            //remove post from Posts table
            db.Posts.Remove(post);

            //save changes in the database
            db.SaveChanges();

            //redirect the user to the Index action to see the updated list of posts for the logged-in staff member
            return RedirectToAction("Index");            
            
        }
    }
}
