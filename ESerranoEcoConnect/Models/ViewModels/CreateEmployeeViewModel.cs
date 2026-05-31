using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESerranoEcoConnect.Models.ViewModels
{
    public class CreateEmployeeViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string Role { get; set; }     
        public ICollection<SelectListItem> Roles { get; set; }

        [Display(Name = "isSuspended")]
        public bool isSuspended { get; set; }

    }
}