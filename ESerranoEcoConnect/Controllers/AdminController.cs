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
    // Stage 3 Task 2.11. Restrict access to the AdminController to only users in the "Admin" role. Use the [Authorize] attribute to specify that only users with the "Admin" role can access the controller.
    [Authorize(Roles = "Admin")]// this controller can be access only by admin role users

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

            var users = db.Users.OrderBy(u => u.dateRegistered).ToList();

            // send the list users to the Index view to display the users in a table
            return View(users);
        }
        // ********************************************
        //  CREATE A NEW USER BY THE ADMIN

        // stage 3 Task 3.2 Create a new user,  mainly Staff or Moderator (Employee. Add a new action method in the AdminController to handle the creation of a new employee. This method should accept a CreateEmployeeViewModel as a parameter and use it to create a new user in the database with the specified role.

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
                    dateRegistered = DateTime.Now
                };
                // create the user in the database with the specified password
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // assign the selected role to the user
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
                isSuspended = staff.isSuspended,
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
                staff.isSuspended = model.isSuspended;
                // update the user's role if it has changed
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
                isSuspended = member.isSuspended,
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
                member.isSuspended = model.isSuspended;
                // update the user's role if it has changed
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
    }
}