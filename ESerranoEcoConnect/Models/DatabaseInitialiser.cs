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


//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

namespace ESerranoEcoConnect.Models
{
    public class DatabaseInitialiser : DropCreateDatabaseAlways<EcoConnectDbContext>
    {
        protected override void Seed(EcoConnectDbContext context)
        {
            // -------------------------------------------------------
            // 1. ROLE MANAGER + USER MANAGER
            // -------------------------------------------------------
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<User>(new UserStore<User>(context));

            // Relax password rules for seeding
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
                RequireNonLetterOrDigit = false
            };

            // -------------------------------------------------------
            // 2. CREATE ROLES
            // -------------------------------------------------------
            string[] roles = { "Admin", "Staff", "Member", "Moderator" };

            foreach (var r in roles)
            {
                if (!roleManager.RoleExists(r))
                    roleManager.Create(new IdentityRole(r));
            }

            // -------------------------------------------------------
            // 3. CREATE USERS (Staff + Members)
            // -------------------------------------------------------

            // ADMIN (is a Staff)
            var admin = new Staff
            {
                UserName = "admin@ecoconnect.com",
                Email = "admin@ecoconnect.com",
                FirstName = "Admin",
                LastName = "User",
                DateRegistered = DateTime.Now,
                IsSuspended = false,
                StaffRole = StaffRole.Admin
            };

            userManager.Create(admin, "admin123");
            userManager.AddToRole(admin.Id, "Admin");

            // MODERATOR (is a Staff)
            var moderator = new Staff
            {
                UserName = "moderator@ecoconnect.com",
                Email = "moderator@ecoconnect.com",
                FirstName = "Post",
                LastName = "Moderator",
                DateRegistered = DateTime.Now,
                IsSuspended = false,
                StaffRole = StaffRole.Moderator
            };

            userManager.Create(moderator, "moderator");
            userManager.AddToRole(moderator.Id, "Moderator");

            // STAFF
            var staff = new Staff
            {
                UserName = "staff@ecoconnect.com",
                Email = "staff@ecoconnect.com",
                FirstName = "Eco",
                LastName = "Staff",
                DateRegistered = DateTime.Now,
                IsSuspended = false,
                StaffRole = StaffRole.Staff
            };

            userManager.Create(staff, "staff");
            userManager.AddToRole(staff.Id, "Staff");

            // MEMBER 1
            var member1 = new Member
            {
                UserName = "member1@ecoconnect.com",
                Email = "member1@ecoconnect.com",
                FirstName = "First",
                LastName = "Member",
                DateRegistered = DateTime.Now,
                IsSuspended = false
            };

            userManager.Create(member1, "member1");
            userManager.AddToRole(member1.Id, "Member");

            // MEMBER 2
            var member2 = new Member
            {
                UserName = "member2@ecoconnect.com",
                Email = "member2@ecoconnect.com",
                FirstName = "Second",
                LastName = "Member",
                DateRegistered = DateTime.Now,
                IsSuspended = false
            };

            userManager.Create(member2, "member2");
            userManager.AddToRole(member2.Id, "Member");

            // SUSPENDED MEMBER
            var suspended = new Member
            {
                UserName = "suspended@ecoconnect.com",
                Email = "suspended@ecoconnect.com",
                FirstName = "Suspended",
                LastName = "Member",
                DateRegistered = DateTime.Now,
                IsSuspended = true
            };

            userManager.Create(suspended, "suspended");
            userManager.AddToRole(suspended.Id, "Member");

            context.SaveChanges();

            // -------------------------------------------------------
            // 4. CATEGORIES
            // -------------------------------------------------------
            var categories = new List<Category>
            {
                new Category { CategoryName = "Sustainable Living" },
                new Category { CategoryName = "Renewable Energy" },
                new Category { CategoryName = "Climate Change" },
                new Category { CategoryName = "Conservation" },
                new Category { CategoryName = "Green Technology" },
                new Category { CategoryName = "Eco-Friendly Products" },
                new Category { CategoryName = "Sustainable Agriculture" },
                new Category { CategoryName = "Environmental Policy" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // -------------------------------------------------------
            // 5. POSTS (created by Staff/Admin)
            // -------------------------------------------------------
            var post1 = new Post
            {
                Title = "10 Tips for Sustainable Living",
                Content = "Discover practical tips to reduce your environmental impact.",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                StaffId = admin.Id,
                IsAnnouncement = false,
                IsEventUpdate = false,
                CategoryId = categories.First(c => c.CategoryName == "Sustainable Living").CategoryId
            };

            var post2 = new Post
            {
                Title = "The Benefits of Renewable Energy",
                Content = "Explore the advantages of renewable energy sources.",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                StaffId = staff.Id,
                IsAnnouncement = false,
                IsEventUpdate = false,
                CategoryId = categories.First(c => c.CategoryName == "Renewable Energy").CategoryId
            };

            context.Posts.Add(post1);
            context.Posts.Add(post2);
            context.SaveChanges();

            // -------------------------------------------------------
            // 6. COMMENTS (Task 9)
            // -------------------------------------------------------
            var comment1 = new Comment
            {
                Content = "Great post! Very helpful.",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PostId = post1.PostId,
                AuthorId = member1.Id,
                IsFlagged = false
            };

            var comment2 = new Comment
            {
                Content = "Thanks for the tips!",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PostId = post2.PostId,
                AuthorId = member2.Id,
                IsFlagged = false
            };

            context.Comments.Add(comment1);
            context.Comments.Add(comment2);

            context.SaveChanges();
        }
    }
}










//namespace ESerranoEcoConnect.Models
//{
//internal class DatabaseInitialiser : DropCreateDatabaseIfModelChanges<EcoConnectDbContext>
//{
//    protected override void Seed(EcoConnectDbContext context)
//    {
//        base.Seed(context);

//        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));// before the if to avoid error of creating multiple Role managers if there are multiple users
//        var userManager = new UserManager<User>(new UserStore<User>(context)); // before the if to avoid error of creating multiple user managers if there are multiple users
//        //*****************************************************
//        //                         ROLES
//        //*****************************************************

//        // added at Stage 3 Task 2 when a IsSuspended Role is detected and gives an error to Admin dashboard list because there are 2 roles in a ussser.
//        //if there are no records stored in the Users table
//        if (!context.Users.Any())
//        {

//            // Ensure only these four roles exist
//            var requiredRoles = new[] { "Admin", "Member", "Staff", "Moderator" };
//            foreach (var r in requiredRoles)
//            {
//                if (!roleManager.RoleExists(r))
//                {
//                    roleManager.Create(new IdentityRole(r));
//                }
//            }

//            //******************************************************
//            //                     USERS
//            //******************************************************

//            //create a new user manager object , to create users-members or staff
//            //and store them in the database in the Users table



//            // *********create an ADMIN user *********
//            // and store it in the database in the Users table

//            // first check if the admin exists in the database
//            if (userManager.FindByName("admin@ecoconnect.com") == null)
//            {
//                // Configure laxe validation logic for  for seeding the admin user
//                userManager.PasswordValidator = new PasswordValidator
//                {
//                    RequiredLength = 1,
//                    RequireNonLetterOrDigit = false,
//                    RequireDigit = false,
//                    RequireLowercase = false,
//                    RequireUppercase = false,
//                };

//                // create the admin staff user and store it in the database
//                var admin = new Staff
//                {
//                    UserName = "admin@ecoconnect.com",
//                    Email = "admin@ecoconnect.com",
//                    FirstName = "Admin",
//                    LastName = "User",
//                    DateRegistered = DateTime.Now,
//                    IsSuspended = false,
//                    StaffRole = StaffRole.Admin

//                };

//                //add admin to the Users table in the database
//                var result = userManager.Create(admin, "admin123");// create the admin user with the password "admin123"
//                                                                   // assign it to the Admin Role
//                userManager.AddToRole(admin.Id, "Admin");// save the admin user in the database
//            }

//            // ======= Create MODERATOR user ====================================

//            // check if the moderator exists
//            if (userManager.FindByName("moderator@ecoconnect.com") == null)
//            {
//                // relax password rules for seeding
//                userManager.PasswordValidator = new PasswordValidator
//                {
//                    RequiredLength = 1,
//                    RequireNonLetterOrDigit = false,
//                    RequireDigit = false,
//                    RequireLowercase = false,
//                    RequireUppercase = false,
//                };

//                // create the moderator user
//                var moderator = new Staff
//                {
//                    UserName = "moderator@ecoconnect.com",
//                    Email = "moderator@ecoconnect.com",
//                    FirstName = "Post",
//                    LastName = "Moderator",
//                    DateRegistered = DateTime.Now,
//                    IsSuspended = false
//                };

//                // create user in DB
//                var result = userManager.Create(moderator, "moderator");

//                // assign Moderator Role
//                if (result.Succeeded)
//                {
//                    userManager.AddToRole(moderator.Id, "Moderator");
//                }
//            }

//            // ======= Create STAFF user ====================================

//            // check if the staff user exists
//            if (userManager.FindByName("staff@ecoconnect.com") == null)
//            {
//                // relax password rules for seeding
//                userManager.PasswordValidator = new PasswordValidator
//                {
//                    RequiredLength = 1,
//                    RequireNonLetterOrDigit = false,
//                    RequireDigit = false,
//                    RequireLowercase = false,
//                    RequireUppercase = false,
//                };

//                // create the staff user
//                var staff = new Staff
//                {
//                    UserName = "staff@ecoconnect.com",
//                    Email = "staff@ecoconnect.com",
//                    FirstName = "Eco",
//                    LastName = "Staff",
//                    DateRegistered = DateTime.Now,
//                    IsSuspended = false
//                };

//                // create user in DB
//                var result = userManager.Create(staff, "staff");

//                // assign Staff Role
//                if (result.Succeeded)
//                {
//                    userManager.AddToRole(staff.Id, "Staff");
//                }
//            }

//            // ======= Create MEMBER users ====================================

//            // Member 1
//            if (userManager.FindByName("member1@ecoconnect.com") == null)
//            {
//                userManager.PasswordValidator = new PasswordValidator
//                {
//                    RequiredLength = 1,
//                    RequireNonLetterOrDigit = false,
//                    RequireDigit = false,
//                    RequireLowercase = false,
//                    RequireUppercase = false,
//                };

//                var member1 = new Member
//                {
//                    UserName = "member1@ecoconnect.com",
//                    Email = "member1@ecoconnect.com",
//                    FirstName = "First",
//                    LastName = "Member",
//                    DateRegistered = DateTime.Now,
//                    IsSuspended = false
//                };

//                var result1 = userManager.Create(member1, "member1");//user Name and password

//                if (result1.Succeeded)
//                {
//                    userManager.AddToRole(member1.Id, "Member");
//                }
//            }

//            // Member 2
//            if (userManager.FindByName("member2@ecoconnect.com") == null)
//            {
//                userManager.PasswordValidator = new PasswordValidator
//                {
//                    RequiredLength = 1,
//                    RequireNonLetterOrDigit = false,
//                    RequireDigit = false,
//                    RequireLowercase = false,
//                    RequireUppercase = false,
//                };

//                var member2 = new Member
//                {
//                    UserName = "member2@ecoconnect.com",
//                    Email = "member2@ecoconnect.com",
//                    FirstName = "Second",
//                    LastName = "Member",
//                    DateRegistered = DateTime.Now,
//                    IsSuspended = false
//                };

//                var result2 = userManager.Create(member2, "member2");

//                if (result2.Succeeded)
//                {
//                    userManager.AddToRole(member2.Id, "Member");
//                }
//            }

//            // ======= Create a  MEMBER user that is suspended ====================================

//            if (userManager.FindByName("suspended@ecoconnect.com") == null)
//            {
//                // relax password rules for seeding
//                userManager.PasswordValidator = new PasswordValidator
//                {
//                    RequiredLength = 1,
//                    RequireNonLetterOrDigit = false,
//                    RequireDigit = false,
//                    RequireLowercase = false,
//                    RequireUppercase = false,
//                };

//                var suspendedMember = new Member
//                {
//                    UserName = "suspended@ecoconnect.com",
//                    Email = "suspended@ecoconnect.com",
//                    FirstName = "Suspended",
//                    LastName = "Member",
//                    DateRegistered = DateTime.Now,
//                    IsSuspended = true //  key part
//                };

//                var result = userManager.Create(suspendedMember, "suspended");//PASSWORD

//                if (result.Succeeded)
//                {
//                    // assign Member Role
//                    userManager.AddToRole(suspendedMember.Id, "Member");

//                    // also assign IsSuspended Role if you want Role-based blocking
//                    if (roleManager.RoleExists("IsSuspended"))
//                    {
//                        userManager.AddToRole(suspendedMember.Id, "IsSuspended");
//                    }
//                }
//            }//end if suspended member




//        //save the users in the data base
//        context.SaveChanges();
//        }
//        //*****************************************************
//        //  seeding the Categories table with some categories
//        //*****************************************************
//        if (!context.Categories.Any())
//        {
//            // create a list of categories to add to the database
//            var categories = new List<Category>
//        {
//            new Category { CategoryName = "Sustainable Living" },
//            new Category { CategoryName = "Renewable Energy" },
//            new Category { CategoryName = "Climate Change" },
//            new Category { CategoryName = "Conservation" },
//            new Category { CategoryName = "Green Technology" },
//            new Category { CategoryName = "Eco-Friendly Products" },
//            new Category { CategoryName = "Sustainable Agriculture" },
//            new Category { CategoryName = "Environmental Policy" }
//        };
//            //add each category to the database
//            foreach (var c in categories)
//            {
//                context.Categories.Add(c);
//            }

//            //save the categories in the database
//            context.SaveChanges();
//        }
//            //*****************************************************
//            //  seeding the Posts table with some posts
//            //*****************************************************
//            if (!context.Posts.Any())
//            {
//            // Get admin ID to assign as author
//            var adminId = context.Users
//                .FirstOrDefault(u => u.UserName == "admin@ecoconnect.com")?.Id;

//            //create a post and add it to the database
//            var post1 = new Post
//            {
//                Title = "10 Tips for Sustainable Living",
//                Content = "Discover practical tips to reduce your environmental impact and live a more sustainable lifestyle.",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                StaffId = adminId, // assign to admin user
//                IsAnnouncement = false,
//                IsEventUpdate = false,
//                CategoryId = context.Categories.FirstOrDefault(c => c.CategoryName == "Sustainable Living")?.CategoryId ?? 0
//            };

//            //create another post and add it to the database
//            var post2 = new Post
//            {
//                Title = "The Benefits of Renewable Energy",
//                Content = "Explore the advantages of renewable energy sources and how they can help combat climate change.",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                StaffId = adminId, // assign to admin user
//                IsAnnouncement = false,
//                IsEventUpdate = false,
//                CategoryId = context.Categories.FirstOrDefault(c => c.CategoryName == "Renewable Energy")?.CategoryId ?? 0
//            };

//            //create another post and add it to the database
//            var post3 = new Post
//            {
//                Title = "Upcoming Eco-Friendly Event",
//                Content = "Join us for an upcoming event focused on eco-friendly practices and sustainability.",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                StaffId = adminId, // assign to admin user
//                IsAnnouncement = true, // this is an announcement post
//                IsEventUpdate = false,
//                CategoryId = context.Categories.FirstOrDefault(c => c.CategoryName == "Environmental Policy")?.CategoryId ?? 0
//            };

//            //create another post and add it to the database
//            var post4 = new Post
//            {
//                Title = "Event Update: Green Tech Conference",
//                Content = "Get the latest updates on the upcoming Green Tech Conference, including speakers and schedule.",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                StaffId = adminId, // assign to admin user
//                IsAnnouncement = false,
//                IsEventUpdate = true, // this is an event update post
//                CategoryId = context.Categories.FirstOrDefault(c => c.CategoryName == "Green Technology")?.CategoryId ?? 0
//            };

//            //create another post and add it to the database
//            var post5 = new Post
//            {
//                Title = "New Eco-Friendly Product Launch",
//                Content = "Introducing our latest eco-friendly product designed to help you reduce your carbon footprint.",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                StaffId = adminId, // assign to admin user
//                IsAnnouncement = true, // this is an announcement post
//                IsEventUpdate = false,
//                CategoryId = context.Categories.FirstOrDefault(c => c.CategoryName == "Eco-Friendly Products")?.CategoryId ?? 0
//            };

//            //create another post and add it to the database
//            var post6 = new Post
//            {
//                Title = "Sustainable Agriculture Practices",
//                Content = "Learn about sustainable agriculture practices that promote environmental health and food security.",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                StaffId = adminId, // assign to admin user 
//                IsAnnouncement = false,
//                IsEventUpdate = false,
//                CategoryId = context.Categories.FirstOrDefault(c => c.CategoryName == "Sustainable Agriculture")?.CategoryId ?? 0
//            };

//            context.Posts.Add(post1);
//            context.Posts.Add(post2);
//            context.Posts.Add(post3);
//            context.Posts.Add(post4);
//            context.Posts.Add(post5);
//            context.Posts.Add(post6);

//            //save the posts in the database
//            context.SaveChanges();
//            }

//    }//end if any users

//}//end class

//}//end namespace