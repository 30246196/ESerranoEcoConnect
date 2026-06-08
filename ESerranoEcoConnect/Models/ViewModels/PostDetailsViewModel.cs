using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESerranoEcoConnect.Models;//added 

namespace ESerranoEcoConnect.Models.ViewModels
{
    public class PostDetailsViewModel
    {
        // the PostId is needed to link the new comment to the correct post when creating a new comment
        public int PostId { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CategoryName { get; set; }
        public string AuthorName { get; set; }

        public List<Comment> Comments { get; set; }

        // for the form Create a new comment
        public string NewCommentContent { get; set; }
    }
}