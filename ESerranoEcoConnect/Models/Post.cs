using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESerranoEcoConnect.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name ="Date Created")]
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        public bool IsAnnouncement { get; set; }

        public bool IsEventUpdate { get; set; }

        //FK to Staff (author)
        [ForeignKey("Staff")]
        public string StaffId { get; set; }
        public Staff Staff { get; set; }
        
                
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public  Category Category { get; set; }

        // NAVIGATION PROPERTIES 
        // A post can have many comments, but a comment can only belong to one post
        public virtual ICollection<Comment> Comments { get; set; }


    }
}