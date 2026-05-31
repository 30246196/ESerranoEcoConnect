using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ESerranoEcoConnect.Models.ViewModels
{
    public class EditStaffViewModel
    {
        [Required]
        [Display(Name = "User ID")]
        public string Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Role")]
        public string Role { get; set; }
            [Display(Name = "isSuspended")]
            public bool isSuspended { get; set; }

    }
}