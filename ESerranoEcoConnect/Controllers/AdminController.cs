using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;//added for DataTable
using System.Data.Entity;//added for Entity Framework
using System.Net;//added for HttpStatusCodeResult
using ESerranoEcoConnect.Models;//added for ApplicationDbContext and ApplicationUser
using ESerranoEcoConnect.Models.ViewModels;//added for AdminDashboardViewModel
using Microsoft.AspNet.Identity;//added for User.Identity.GetUserId()
using Microsoft.AspNet.Identity.EntityFramework;//added for UserManager and RoleManager
using System.Threading.Tasks;//added for async Task<ActionResult>


namespace ESerranoEcoConnect.Controllers
{
    // change the inheritance from Controller to AccountController
    // to get access to all the methods of AcountController, such as Register, Login, 
    public class AdminController : AccountController // controler inherits from AccountController
    {
        // constructor that calls the base constructor of AccountController
        public AdminController():base()
        {

        }

        // constructor that takes ApplicationUserManager and ApplicationRoleManager as parameters and calls the base constructor of AccountController
        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
            : base(userManager, signInManager)
        {
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
    }
}