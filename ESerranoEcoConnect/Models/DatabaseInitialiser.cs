using System;
using System.Collections.Generic;
using System.IO;// added for the Path class?
using System.Linq;
using System.Text;
//using Microsoft.Analytics.Interfaces; // appear when created the class but not used, so I commented out
//using Microsoft.Analytics.Types.Sql;
using System.Data.Entity; //added for the DbContext class
using Microsoft.AspNet.Identity.EntityFramework; //added for the IdentityDbContext class
using Microsoft.AspNet.Identity;//added for the UserManager class

namespace ESerranoEcoConnect.Models
{
    internal class DatabaseInitialiser : DropCreateDatabaseIfModelChanges<EcoConnectDbContext>
    {
        protected override void Seed(EcoConnectDbContext context)
        {
            base.Seed(context);

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));// before the if to avoid error of creating multiple role managers if there are multiple users

            //if there are no records stored in the Users table
            if (!context.Users.Any())
            {
                //create some roles:Admin, Staff, manager, Member and IsSuspended and store them in the database in the Roles table

                //create a new role manager object
                //RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                //If Admin role does not exist
                //create the Admin role and store it in the database
                if (!roleManager.RoleExists("Admin"))
                {
                    roleManager.Create(new IdentityRole("Admin"));
                }

                //If Staff role does not exist
                //create the Staff role and store it in the database
                if (!roleManager.RoleExists("Staff"))
                {
                    roleManager.Create(new IdentityRole("Staff"));
                }

                //If Manager role does not exist
                //create the Manager role and store it in the database
                if (!roleManager.RoleExists("Manager"))
                {
                    roleManager.Create(new IdentityRole("Manager"));
                }

                //If Member role does not exist
                //create the Member role and store it in the database
                if (!roleManager.RoleExists("Member"))
                {
                    roleManager.Create(new IdentityRole("Member"));
                }

                //create a new role for membes that are suspended and not allowed to log in to the system or comment
                if (!roleManager.RoleExists("IsSuspended"))
                {
                    roleManager.Create(new IdentityRole("IsSuspended"));
                }

                // save the roles in the data base
                context.SaveChanges();

            }

            // ======= Create Users ====================================

            //create a new user manager object , to create users-members or staff
            //and store them in the database in the Users table

            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));

            // *********create an ADMIN user *********
            // and store it in the database in the Users table

            // first check if the admin exists in the database
            if (userManager.FindByName("admin@ecoconnect.com") == null)
            {
                // Configure laxe validation logic for  for seeding the admin user
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 1,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };

                // create the admin staff user and store it in the database
                var admin = new Staff
                {
                    UserName = "admin@ecoconnect.com",
                    Email = "admin@ecoconnect.com",
                    FirstName = "Admin",
                    LastName = "User",
                    dateRegistered = DateTime.Now,
                    isSuspended = false,
                    StaffRole = StaffRole.Admin

                };

                //add admin to the Users table in the database
                var result = userManager.Create(admin, "admin123");// create the admin user with the password "admin123"
                // assign it to the Admin role
                userManager.AddToRole(admin.Id, "Admin");// save the admin user in the database
            }

            // ======= Create MANAGER user ====================================

            // check if the manager exists
            if (userManager.FindByName("manager@ecoconnect.com") == null)
            {
                // relax password rules for seeding
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 1,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };

                // create the manager user
                var manager = new Staff
                {
                    UserName = "manager@ecoconnect.com",
                    Email = "manager@ecoconnect.com",
                    FirstName = "System",
                    LastName = "Manager",
                    dateRegistered = DateTime.Now,
                    isSuspended = false
                };

                // create user in DB
                var result = userManager.Create(manager, "manager");

                // assign Manager role
                if (result.Succeeded)
                {
                    userManager.AddToRole(manager.Id, "Manager");
                }
            }

            // ======= Create STAFF user ====================================

            // check if the staff user exists
            if (userManager.FindByName("staff@ecoconnect.com") == null)
            {
                // relax password rules for seeding
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 1,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };

                // create the staff user
                var staff = new Staff
                {
                    UserName = "staff@ecoconnect.com",
                    Email = "staff@ecoconnect.com",
                    FirstName = "Eco",
                    LastName = "Staff",
                    dateRegistered = DateTime.Now,
                    isSuspended = false
                };

                // create user in DB
                var result = userManager.Create(staff, "staff");

                // assign Staff role
                if (result.Succeeded)
                {
                    userManager.AddToRole(staff.Id, "Staff");
                }
            }

            // ======= Create MEMBER users ====================================

            // Member 1
            if (userManager.FindByName("member1@ecoconnect.com") == null)
            {
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 1,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };

                var member1 = new Member
                {
                    UserName = "member1@ecoconnect.com",
                    Email = "member1@ecoconnect.com",
                    FirstName = "First",
                    LastName = "Member",
                    dateRegistered = DateTime.Now,
                    isSuspended = false
                };

                var result1 = userManager.Create(member1, "member1");

                if (result1.Succeeded)
                {
                    userManager.AddToRole(member1.Id, "Member");
                }
            }

            // Member 2
            if (userManager.FindByName("member2@ecoconnect.com") == null)
            {
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 1,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };

                var member2 = new Member
                {
                    UserName = "member2@ecoconnect.com",
                    Email = "member2@ecoconnect.com",
                    FirstName = "Second",
                    LastName = "Member",
                    dateRegistered = DateTime.Now,
                    isSuspended = false
                };

                var result2 = userManager.Create(member2, "member2");

                if (result2.Succeeded)
                {
                    userManager.AddToRole(member2.Id, "Member");
                }
            }

            // ======= Create SUSPENDED MEMBER user ====================================

            if (userManager.FindByName("suspended@ecoconnect.com") == null)
            {
                // relax password rules for seeding
                userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 1,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };

                var suspendedMember = new Member
                {
                    UserName = "suspended@ecoconnect.com",
                    Email = "suspended@ecoconnect.com",
                    FirstName = "Suspended",
                    LastName = "Member",
                    dateRegistered = DateTime.Now,
                    isSuspended = true // 🔥 key part
                };

                var result = userManager.Create(suspendedMember, "suspended");

                if (result.Succeeded)
                {
                    // assign Member role
                    userManager.AddToRole(suspendedMember.Id, "Member");

                    // also assign IsSuspended role if you want role-based blocking
                    if (roleManager.RoleExists("IsSuspended"))
                    {
                        userManager.AddToRole(suspendedMember.Id, "IsSuspended");
                    }
                }
            }//end if suspended member


        }//end if any users
    }//end class

}//end namespace