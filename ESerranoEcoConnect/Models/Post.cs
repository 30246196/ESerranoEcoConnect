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

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }

        //FK to Staff (author)
        [Required]
        public string StaffId { get; set; }
        public Staff Staff { get; set; }

        [Required]
        public bool isAnnouncement { get; set; }

        [Required]
        public bool isEventUpdate { get; set; }

        // NAVIGATION PROPERTIES
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public  Category Category { get; set; }

        //[ForeignKey("Staff")]
        //public string Id { get; set; }
        //public  Staff Staff { get; set; }
    }
}