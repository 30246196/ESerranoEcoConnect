using ESerranoEcoConnect.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;

namespace ESerranoEcoConnect.Controllers
{
    [Authorize(Roles = "Moderator")]// This attribute ensures that only users with the "Moderator" role can access the actions in this controller

    public class ModeratorController : Controller
    {
        //create an instance of the database context to interact with the database
        private EcoConnectDbContext db = new EcoConnectDbContext();

        // GET: Moderator
        [Authorize(Roles = "Moderator")]// This attribute ensures that only users with the "Moderator" role can access this action
        public ActionResult Index()
        {
            return View();
        }

        // GET: Moderator/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Moderator/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Moderator/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Moderator/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Moderator/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Moderator/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Moderator/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //*******************************************************************************************************
        //                            CATEGORY                MODERATORS 
        //*******************************************************************************************************

        // Stage 8 Task 4. Moderator can manage categories:

        // view all categories,
        // create a new category,
        // edit a category,
        // delete a category

        // GET: Categories
        public ActionResult ViewAllCategories()
        {

            //return the ViewAllCategories view and pass the list of categories to it to display them in a table
            return View(db.Categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult CategoryDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find category by id in Categories table in the db
            var category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            //send the category to the CategoryDetails view to display the details of the category
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult CreateCategory()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory([Bind(Include = "CategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                //add the new category to the Categories table in the db
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("ViewAllCategories");
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find category by id in Categories table in the db
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            //send the category to the EditCategory view to edit the category
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory([Bind(Include = "CategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                //update the category in the Categories table in the db
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewAllCategories");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find category by id in Categories table in the db
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            //send the category to the DeleteCategory view to confirm the deletion of the category
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategoryConfirmed(int id)
        {
            //find category by id in Categories table in the db
            var category = db.Categories.Find(id);

            //remove the category from the Categories table in the db
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("ViewAllCategories");
        }

        // Dispose the database context when the controller is disposed to free up resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //*******************************************************************************************************
        //                      ROLES
        //*******************************************************************************************************

        // Implement that Moderator can view all posts and delete inappropriate posts
        [Authorize(Roles = "Moderator")]// This attribute ensures that only users with the "Moderator" role can access this action
        public ActionResult ViewAllPosts()
        {
            //get all posts from the Posts table in the db and pass them to the ViewAllPosts view to display them in a table
            List<Post> posts = db.Posts
                .Include(p=>p.Category)
                .Include(p=>p.Staff)
                .ToList();

            //return the ViewAllPosts view and pass the list of posts to it to display them in a table
            return View(posts);
        }

    }
}
