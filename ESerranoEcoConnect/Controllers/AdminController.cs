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
        public AdminController():base()
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
    }
}