using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using System.ComponentModel.DataAnnotations.Schema;// added for the [NotMapped] attribute
using System.Web;//added for HttpPostedFileBase
using Microsoft.AspNet.Identity.Owin;//added for GetOwinContext() method
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;//added for the .Where() method

namespace ESerranoEcoConnect.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public abstract class User : IdentityUser // Stage 2. Task2.3. add Abstract User class that inherits from IdentityUser, and then have Member and Staff inherit from User. This way we can have common properties for both types of users in the User class, and specific properties for each type of user in the Member and Staff classes.
    {
        // Stage 2.2 and 2.4: Extending the IdentityUser class with additional properties
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public bool isSuspended{ get; set; }
        public DateTime dateRegistered { get; set; }
        public IdentityUserRole role { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        //TODO: Add a method to get the user's role as a string

        // NEED the ApplicationUserManager to get the usr's current role, but can't inject it into the User class, so we have to get it from the OwinContext
        private ApplicationUserManager userManager;

        // the currentRole property is not mapped as a field in the user's table
        // I need it to get the current role that the user is logged in with, so I can display it in the profile page and use it for authorization checks in the views

        [NotMapped]// This property is not mapped to the database, it's just for convenience
        public string CurrentRole
        {
            get
            {              
                if (userManager == null)
                {
                    userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    
                }

                return userManager.GetRoles(Id).Single(); // Assuming a user has only one role, otherwise you might want to return a list of roles
            }
        }
    }


}