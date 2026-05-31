using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESerranoEcoConnect.Models.ViewModels
{
    public class EditMemberViewModel
    {
        
        [Required]
        [Display(Name = "First Name")]  
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
         [Display(Name = "isSuspended")]
         public bool isSuspended { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
        

    }
}