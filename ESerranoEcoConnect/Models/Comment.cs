using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ESerranoEcoConnect.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Moderate by Moderator or Admin
        public bool IsFlagged { get; set; }

        //FK to Member (author)
        [ForeignKey("Author")]
        public string AuthorId { get; set; }//string because inherits from IdentityUser. has to link with the user, member
        public Member Author { get; set; }// Member writes the comments


        // FK to Post
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

       
                   
    }
}