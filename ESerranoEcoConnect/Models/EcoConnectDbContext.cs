using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using ESerranoEcoConnect.Models;//added
using System.Data.Entity;// added for the Database.SetInitializer() method

namespace ESerranoEcoConnect.Models
{
    public class EcoConnectDbContext : IdentityDbContext<User>
    {
        // Stage 4: Add DbSet properties for each of the models

        // Tables
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }        
        public DbSet<Member> Members { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<ContactForm> ContactForms { get; set; }// added in Stage 11: Contact Form

        public EcoConnectDbContext()
            : base("EcoConnectConnectionV2", throwIfV1Schema: false)//changed first in <connectionStrings>... name=Defaultconnection to name= EcoConnectConnection in Web.config
        {
            Database.SetInitializer(new DatabaseInitialiser());
        }

        public static EcoConnectDbContext Create()
        {
            return new EcoConnectDbContext();
        }

        // ADDED: to avoid "multiple cascade paths" and errors from changing to capitals the attributes.
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Comment → Post (one to many) SIN cascade delete
            modelBuilder.Entity<Comment>()
                .HasRequired(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .WillCascadeOnDelete(false);

            // Comment → Member (one to many) SIN cascade delete           
            modelBuilder.Entity<Comment>()
                .HasRequired(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .WillCascadeOnDelete(false);

            // Post → Staff (one to many) CON cascade delete (optional)
            modelBuilder.Entity<Post>()
                .HasRequired(m => m.Staff)
                .WithMany(c => c.Posts)
                .HasForeignKey(c => c.StaffId)
                .WillCascadeOnDelete(false);

            // CATEGORY → POSTS (one-to-many)
            // (No cascade delete needed, EF handles it safely)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Posts)
                .WithRequired(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .WillCascadeOnDelete(false);
        }
    }
}