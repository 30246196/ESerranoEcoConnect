using ESerranoEcoConnect.Models;//added for ApplicationDbContext and ApplicationUser
using ESerranoEcoConnect.Models.ViewModels;//added for AdminDashboardViewModel
using Microsoft.AspNet.Identity;//added for User.Identity.GetUserId()
using Microsoft.AspNet.Identity.EntityFramework;//added for UserManager and RoleManager
using System;
using System.Collections.Generic;
using System.Data;//added for DataTable
using System.Data.Entity;//added for Entity Framework
using System.Linq;
using System.Net;//added for HttpStatusCodeResult
using System.Threading.Tasks;//added for async Task<ActionResult>
using System.Web;
using System.Web.Mvc;


namespace ESerranoEcoConnect.Controllers
{
    // Stage 3 Task 2.11. Restrict access to the AdminController to only users in the "Admin" Role. Use the [Authorize] attribute to specify that only users with the "Admin" Role can access the controller.
    [Authorize(Roles = "Admin")]// this controller can be access only by admin Role users

    // change the inheritance from Controller to AccountController
    // to get access to all the methods of AcountController, such as Register, Login, 
    public class AdminController : AccountController // controler inherits from AccountController
    {

        //Stage 3 Task 1: Create Admin Dashboard View
        // constructor that calls the base constructor of AccountController
        public AdminController() : base()
        {

        }

        // constructor that takes ApplicationUserManager and ApplicationRoleManager as parameters and calls the base constructor of AccountController
        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager)
        {
        }

        // Stage 3 Task 2 admin can view users
        // addd an instance of EcoConnectDbContext to access the database
        private EcoConnectDbContext db = new EcoConnectDbContext();


        // GET: Admin

        // Stage 3 Task 2.3: Admin can view users. Update Index action to retrieve the list of users from the database and display them in the view.

        public ActionResult Index()
        {
            // get all users from the database order by registration date and pass them to the view

            var users = db.Users.OrderBy(u => u.DateRegistered).ToList();

            // send the list users to the Index view to display the users in a table
            return View(users);
        }
        // ********************************************
        //  CREATE A NEW USER BY THE ADMIN

        // stage 3 Task 3.2 Create a new user,  mainly Staff or Moderator (Employee. Add a new action method in the AdminController to handle the creation of a new employee. This method should accept a CreateEmployeeViewModel as a parameter and use it to create a new user in the database with the specified Role.

        [HttpGet]
        public ActionResult CreateEmployee()
        {
            CreateEmployeeViewModel newUser = new CreateEmployeeViewModel();

            // get all the roles from the database and store them in a selectList Item. so, roles can be dispalyed ina dropdown list.
            var roles = db.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            //assign the roles to roles property of the model(employee or member)
            newUser.Roles = roles;

            // display 
            return View(newUser);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            // check if the model state is valid before creating the user
            if (ModelState.IsValid)
            {
                // create a new user with the provided information
                var user = new Staff
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateRegistered = DateTime.Now
                };
                // create the user in the database with the specified password
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // assign the selected Role to the user
                    await UserManager.AddToRoleAsync(user.Id, model.Role);
                    // redirect to the admin dashboard after successful creation
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // if there are errors, add them to the ModelState and display them in the view
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            // if we got this far, something failed, redisplay form with errors
            // get all the roles from the database and store them in a selectList Item. so, roles can be dispalyed ina dropdown list.
            var roles = db.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            //assign the roles to roles property of the model(staff, moderator, admin or member)
            model.Roles = roles;
            return View(model);

        }

        //************************************************
        // EDIT STAFF BY ADMIN

        // Stage 3 task 4. Admin can edit staff and memebers details. Add an Edit action method in the AdminController to allow the admin to edit the details of staff and members. This method should accept the user ID as a parameter, retrieve the user from the database, and display their details in a view for editing.
        [HttpGet]
        public ActionResult EditStaff(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find the staff in the databbase by the id and pass it to the view
            var staff = db.Users.OfType<Staff>().FirstOrDefault(s => s.Id == id);
            if (staff == null)
            {
                return HttpNotFound();
            }

            //send the staff details to the view to display them in a form for editing
            return View(new EditStaffViewModel
            {
                // Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = staff.Email,
                isSuspended = staff.IsSuspended,
                Role = db.Roles.Where(r => r.Users.Any(u => u.UserId == staff.Id)).Select(r => r.Name).FirstOrDefault()
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditStaff(string id, [Bind(Include = "FirstName,LastName,Email,isSuspended,Role")] EditStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                // find the staff in the database by the id
                var staff = db.Users.OfType<Staff>().FirstOrDefault(s => s.Id == model.Id);
                if (staff == null)
                {
                    return HttpNotFound();
                }
                // update the staff details with the provided information
                staff.FirstName = model.FirstName;
                staff.LastName = model.LastName;
                staff.Email = model.Email;
                staff.IsSuspended = model.isSuspended;
                // update the user's Role if it has changed
                var currentRole = db.Roles.Where(r => r.Users.Any(u => u.UserId == staff.Id)).Select(r => r.Name).FirstOrDefault();
                if (currentRole != model.Role)
                {
                    await UserManager.RemoveFromRoleAsync(staff.Id, currentRole);
                    await UserManager.AddToRoleAsync(staff.Id, model.Role);
                }
                // save the changes to the database
                db.Entry(staff).State = EntityState.Modified;
                await db.SaveChangesAsync();
                // redirect to the admin dashboard after successful update
                return RedirectToAction("Index", "Admin");
            }
            // if we got this far, something failed, redisplay form with errors
            return View(model);
        }

        [HttpGet]
        public ActionResult EditMember(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Member member = db.Users.OfType<Member>().FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                return HttpNotFound();
            }

            //send memeber's details to the view to display them in a form for editing

            return View(new EditMemberViewModel
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                isSuspended = member.IsSuspended,
                Role = db.Roles.Where(r => r.Users.Any(u => u.UserId == member.Id)).Select(r => r.Name).FirstOrDefault(),

            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditMember(string id, [Bind(Include = "FirstName,LastName,Email,isSuspended,Role")] EditMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                // find the member in the database by the id
                var member = db.Users.OfType<Member>().FirstOrDefault(m => m.Id == id);
                if (member == null)
                {
                    return HttpNotFound();
                }
                // update the member details with the provided information
                member.FirstName = model.FirstName;
                member.LastName = model.LastName;
                member.Email = model.Email;
                member.IsSuspended = model.isSuspended;
                // update the user's Role if it has changed
                var currentRole = db.Roles.Where(r => r.Users.Any(u => u.UserId == member.Id)).Select(r => r.Name).FirstOrDefault();
                if (currentRole != model.Role)
                {
                    await UserManager.RemoveFromRoleAsync(member.Id, currentRole);
                    await UserManager.AddToRoleAsync(member.Id, model.Role);
                }
                // save the changes to the database
                db.Entry(member).State = EntityState.Modified;
                await db.SaveChangesAsync();
                // redirect to the admin dashboard after successful update
                return RedirectToAction("Index", "Admin");
            }
            // if we got this far, something failed, redisplay form with errors
            return View(model);

        }
        //************************************************
        // DETAILS OF A USER BY ADMIN
        //************************************************
        // Stage 3 task 5. Admin can view details of a user.
        // Add a Details action method in the AdminController to allow the admin to view the details of a specific user.
        // This method should accept the user ID as a parameter, retrieve the user from the database, and display their details in a view.
        [HttpGet]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            if (user is Staff)
            {
                return View("DetailsStaff", (Staff)user);
            }

            else if (user is Member)
            {
                return View("DetailsMember", (Member)user);
            }


            // send the user details to the view to display them
            return View(user);

        }

        // ************************************************
        // CREATE A NEW ROLE BY THE ADMIN
        //************************************************
        // Stage 3 Task 6. Admin can create new roles.
        // Add a CreateRole action method in the AdminController to allow the admin to create new roles.
        // This method should accept a Role name as a parameter, create a new Role in the database, and redirect the admin back to the dashboard.

        [HttpGet]
        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                //get the Role manager to manage the roles in the database
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

                //making sure that there are no duplicates roles stored in the database
                if (roleManager.RoleExists(model.RoleName))
                {
                    //create and save the new Role in the database
                    roleManager.Create(new IdentityRole(model.RoleName));

                    return RedirectToAction("Index", "Admin");
                }

            }
            // if we got this far, something failed, redisplay form with errors
            return View(model);
        }

        //************************************************
        // CHANGE USER'S ROLE BY THE ADMIN
        //************************************************
        // Stage 3 Task 7. Admin can change a user's Role.
        // Add a ChangeRole action method in the AdminController to allow the admin to change the Role of a user.
        // This method should accept the user ID and the new Role as parameters, update the user's Role in the database, and redirect the admin back to the dashboard.

        // GEt method to display the form for changing the user's Role
        [HttpGet]
        public async Task<ActionResult> ChangeRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // cannot change your own Role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Admin"); 
            }

            //get user id
            User user = await UserManager.FindByIdAsync(id);

            // get the current Role of the user
            string oldRole = (await UserManager.GetRolesAsync(id)).Single();


            // get all the roles from the database and store them in a selectList Item. so, roles can be dispalyed ina dropdown list.
            var items = db.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name,
                Selected = r.Name == oldRole
            }).ToList();

            //build the ChangeRoleViewModel to pass it to the view
            return View(new ChangeRoleViewModel
            {
                Username=user.UserName,
                OldRole = oldRole,
                Roles = items
            });           
           
        }

        // POST method to handle the form submission for changing the user's Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, [Bind(Include="Role")]ChangeRoleViewModel model)
        {
            // cannot change your own Role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                //get the user id
                User user = await UserManager.FindByIdAsync(id);// get user id

                //get user's current Role
                string oldRole = (await UserManager.GetRolesAsync(id)).Single();

                //if current Role is the same as the new Role, redirect to the admin dashboard without making any changes
                if (oldRole == model.Role)
                {
                    return RedirectToAction("Index", "Admin");
                }

                //remove user from the old Role and add them to the new Role
                await UserManager.RemoveFromRoleAsync(user.Id, oldRole);

                //now add the user to the new Role
                await UserManager.AddToRoleAsync(user.Id, model.Role);

                //if the user was suspended then issuspended the user and save the changes to the database

                if (user.IsSuspended)
                {
                    user.IsSuspended = true;

                    // save the changes to the database
                    await UserManager.UpdateAsync(user);
                }
                return RedirectToAction("Index", "Admin");

            }

            // if we got this far, something failed, redisplay form with errors
            return View(model);
        }

        //************************************************
        // DELETE USER ACCOUNT BY THE ADMIN
        //************************************************
        // Stage3. Task 8 Admin can delete user accounts.
        // Add a Delete action method in the AdminController to allow the admin to delete user accounts. This method should accept the user ID as a parameter, delete the user from the database, and redirect the admin back to the dashboard.

        ////GET: Users/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    // cannot delete your own account
        //    if (id == User.Identity.GetUserId())
        //    {
        //        return RedirectToAction("Index", "Admin");
        //    }

        //    //find the user id in the db and pass it to the view to display the user details before confirming the deletion
        //    var user = await UserManager.FindByIdAsync(id);

        //    //if the user is not found, return a 404 error
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    //Delete user
        //    await UserManager.DeleteAsync(user);

        //    return RedirectToAction("Index", "Admin");
        //}

        // there were some conflicts once we introduced posts and comments to delete an user because of the cascade delete, so I decided to just suspend the user instead of deleting them, and then they won't be able to log in or access any of the features of the website, but their posts and comments will still be visible on the website.
        //************************************************
        // DELETE USER ACCOUNT BY THE ADMIN
        //************************************************

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // cannot delete your own account
            if (id == User.Identity.GetUserId())
                return RedirectToAction("Index", "Admin");

           // find the user
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound();

            // Show confirmation page
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // cannot delete your own account
            if (id == User.Identity.GetUserId())
                return RedirectToAction("Index", "Admin");

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return HttpNotFound();

            // ---------------------------------------------
            // 1. DELETE COMMENTS WRITTEN BY THIS USER
            // ---------------------------------------------
            var userComments = db.Comments.Where(c => c.AuthorId == user.Id).ToList();

            if (userComments.Any())
            {
                db.Comments.RemoveRange(userComments);
                db.SaveChanges();
            }

            // ---------------------------------------------
            // 2. DELETE POSTS IF THE USER IS STAFF
            // ---------------------------------------------
            if (user is Staff)
            {
                // Get all posts created by this staff member
                var staffPosts = db.Posts
                    .Where(p => p.StaffId == user.Id)
                    .ToList();

                if (staffPosts.Any())
                {
                    // Extract PostIds as primitive integers (EF requires this)
                    var staffPostIds = staffPosts
                        .Select(p => p.PostId)
                        .ToList();

                    // First delete comments on those posts
                    var postComments = db.Comments
                        .Where(c => staffPostIds.Contains(c.PostId))
                        .ToList();

                    if (postComments.Any())
                    {
                        db.Comments.RemoveRange(postComments);
                        db.SaveChanges();
                    }

                    // Now delete the posts
                    db.Posts.RemoveRange(staffPosts);
                    db.SaveChanges();
                }
            }

            // ---------------------------------------------
            // 3. DELETE THE USER
            // ---------------------------------------------
            var result = await UserManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error deleting user: " + string.Join("; ", result.Errors));
                return View("Error");
            }

            return RedirectToAction("Index", "Admin");
        }

    }
}