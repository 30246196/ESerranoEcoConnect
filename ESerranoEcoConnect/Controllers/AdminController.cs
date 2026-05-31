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
                    return RedirectToAction("Index","Admin");
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

    }
}