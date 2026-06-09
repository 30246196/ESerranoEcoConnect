using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using ESerranoEcoConnect.Models;

namespace ESerranoEcoConnect.Models
{        
    public class ContactForm
    {
        [Key]
        public int ContactFormId { get; set; }

        // Nullable: anonymous users have no UserId
                       
        public string UserId { get; set; }

        //public virtual User User { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(2000, ErrorMessage = "The message cannot exceed 2000 characters.")]       
        public string Message { get; set; }

        [Required]
        public DateTime SentAt { get; set; }
    }    
}
