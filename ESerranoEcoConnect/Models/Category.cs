using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESerranoEcoConnect.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        // NAVIGATION PROPERTIES
       // public <Post> Posts { get; set; }
        public virtual ICollection<Post> Posts { get; set; }// in Entity Framework, virtual allows for lazy loading of related data. It creates a proxy class that overrides the navigation property and loads the related data from the database when accessed for the first time.
    }
}