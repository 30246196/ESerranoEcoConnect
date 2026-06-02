using ESerranoEcoConnect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESerranoEcoConnect.Controllers
{
    
    public class StaffController : Controller
    {
        private EcoConnectDbContext context = new EcoConnectDbContext();

        // GET:  Posts for the logged-in staff member
        [Authorize(Roles = "Staff")]
        public ActionResult Index()
        {
            // select the posts for the logged-in staff member
            string staffId = User.Identity.GetUserId();

            // Get all posts for the logged-in staff member, include category
            var posts = context.Posts
                .Include("Category")
                .Where(p => p.StaffId == staffId) // Filter posts by the logged-in staff member
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(posts.ToList());
        }

        // GET: Staff/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Staff/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Staff/Create
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

        // GET: Staff/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Staff/Edit/5
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

        // GET: Staff/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Staff/Delete/5
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
    }
}
