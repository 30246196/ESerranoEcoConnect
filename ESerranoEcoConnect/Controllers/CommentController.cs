using ESerranoEcoConnect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;// added for db context
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESerranoEcoConnect.Controllers
{
    //Only registered Members are allowed to create,edit or delete their own coments.
    // Admins and Moderators can edit or delete any comment.
    // Comments can be flagged by users and will be reviewed by Moderators or Admins for appropriate action.
    // Comments are displayed in chronological order under the associated post, with the most recent comments appearing first.
    // suspended users cannot create or edit comments, but can view existing comments.

    //[Authorize(Roles = "Member")]
    public class CommentController : Controller
    {
        //access to database context
        private EcoConnectDbContext db = new EcoConnectDbContext();

        // method IsUserSuspended to check if the user is suspended
        private bool IsUserSuspended()
        {
            var userId = User.Identity.GetUserId();
            var member = db.Users.OfType<Member>().FirstOrDefault(u => u.Id == userId);
            return member != null && member.IsSuspended;
        }

        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }        

        // GET: Comment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Comment/Create
        public ActionResult CreateComment()
        {
            return View();
        }

        // POST: Comment/Create

        // CREATE A COMMENT BY MEMBERS
        // Only non-suspended members can create comments.
        // Suspended users will be redirected to an error page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")] //members and suspended members

        public ActionResult CreateComment(int PostId, string NewCommentContent)
        {
            // Check if the user is suspended before allowing comment creation
            if (IsUserSuspended())
            {
                return new HttpStatusCodeResult(403, "Your account is suspended. You cannot comment.");
            }

            // check the comment is not empty
            if (string.IsNullOrWhiteSpace(NewCommentContent))
                return new HttpStatusCodeResult(400, "Comment cannot be empty.");
            

            //instance a new comment object and populate its properties
            var comment = new Comment
            {
                PostId = PostId,
                Content = NewCommentContent,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsFlagged = false, //new comments are not flagged by default
                AuthorId = User.Identity.GetUserId()// set the author to the current logged in user
            };

            // Add the comment to the database and save changes
            db.Comments.Add(comment);
            db.SaveChanges();

            // Redirect to the post details page after successful comment creation
            {
                return RedirectToAction("Details", "Post", new { id = PostId });
            }
        }

        // GET: Comment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Comment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Details", "Post", new {id =id});
            }
            catch
            {
                return RedirectToAction("Details", "Post", new { id = id });
            }
        }

        // GET: Comment/Delete/5
        public ActionResult Delete(int id)
        {
            return RedirectToAction("Details", "Post", new { id = id });
        }

        // POST: Comment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Details", "Post", new { id = id });
            }
            catch
            {
                return RedirectToAction("Details", "Post", new { id = id });
            }
        }
    }
}
