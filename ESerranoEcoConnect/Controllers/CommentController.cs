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

        // method IsUserSuspended to check if the user is suspended, although suspended members are not allowed to log in.
        private bool IsUserSuspended()
        {
            var userId = User.Identity.GetUserId();
            var member = db.Users.OfType<Member>().FirstOrDefault(u => u.Id == userId);
            return member != null && member.IsSuspended;
        }

        // SHOW COMMENTS FROM THE LOGGED-IN MEMBER
        // Stage 9: Task: the system allow Members to edit their own comments
        [Authorize(Roles = "Member")] // added Stage 9;
        // GET: Comment
        public ActionResult Index()
        {
            // select the comments from the logged-in member
            string memberId = User.Identity.GetUserId();

            //Get all comments from the logged-in member
            var comments = db.Comments
                .Include(c => c.Post.Category)
                .Where(c => c.AuthorId == memberId)// filter by the logged-in user
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(comments.ToList());
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

        // Stage 9: A member can edit their own comment
        // GET: Comment/EditMyComment
        public ActionResult EditMyComment(int? id)// create a nullable int parameter for the comment id
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            // find a comment in the Comments table with the given id,include the author
            Comment myComment = db.Comments.Find(id);

            // if comment does not exist, return 404 error
            if (myComment == null)
            { 
                return HttpNotFound();
            }

            // get a list of all the attributes to edit in a comment, as Content

            return View(myComment);
        }
        // POST: comment/EditMyComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public ActionResult EditMyComment(int id, Comment updatedComment)
        {
            // Prevent suspended users from editing comments
            if (IsUserSuspended())
            {
                return new HttpStatusCodeResult(403, "Your account is suspended. You cannot edit comments.");
            }

            // Retrieve the existing comment from the database
            var existingComment = db.Comments.Find(id);

            if (existingComment == null)
            {
                return HttpNotFound();
            }

            // Ensure the logged‑in member is the author of the comment
            var currentUserId = User.Identity.GetUserId();
            if (existingComment.AuthorId != currentUserId)
            {
                return new HttpStatusCodeResult(403, "You are not authorised to edit this comment.");
            }

            // Validate the new content
            if (string.IsNullOrWhiteSpace(updatedComment.Content))
            {
                ModelState.AddModelError("Content", "Comment cannot be empty.");
                return View(existingComment); // Return the original model so the user can correct it
            }

            // Apply the update
            existingComment.Content = updatedComment.Content;
            existingComment.UpdatedAt = DateTime.Now;

            db.SaveChanges();

            // Redirect back to the post the comment belongs to
            return RedirectToAction("Details", "Post", new { id = existingComment.PostId });
        }


        // GET: Comment/DeleteMyComment/5
        [Authorize(Roles = "Member")]
        public ActionResult DeleteMyComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // Retrieve the comment
            var comment = db.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            // Check ownership
            var currentUserId = User.Identity.GetUserId();
            if (comment.AuthorId != currentUserId)
            {
                return new HttpStatusCodeResult(403, "You are not authorised to delete this comment.");
            }

            // Check suspension
            if (IsUserSuspended())
            {
                return new HttpStatusCodeResult(403, "Your account is suspended. You cannot delete comments.");
            }

            return View(comment);
        }


        // POST: Comment/DeleteMyComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public ActionResult DeleteMyCommentConfirmed(int id)
        {
            // Retrieve the comment
            var comment = db.Comments.Find(id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            // Check ownership
            var currentUserId = User.Identity.GetUserId();
            if (comment.AuthorId != currentUserId)
            {
                return new HttpStatusCodeResult(403, "You are not authorised to delete this comment.");
            }

            // Check suspension
            if (IsUserSuspended())
            {
                return new HttpStatusCodeResult(403, "Your account is suspended. You cannot delete comments.");
            }

            // Store PostId for redirect
            int postId = comment.PostId;

            // Delete the comment
            db.Comments.Remove(comment);
            db.SaveChanges();

            // Add success message
            TempData["SuccessMessage"] = "Your comment has been deleted successfully.";

            // Redirect back to the post
            return RedirectToAction("Index", "Comment");
        }

    }
}
