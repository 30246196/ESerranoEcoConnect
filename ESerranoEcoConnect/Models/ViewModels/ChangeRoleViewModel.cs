using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESerranoEcoConnect.Models.ViewModels
{
    public class ChangeRoleViewModel
    {
        //new class to the ViewModels folder named ChangeRoleViewModel, storing the users' username, old role, new role, and a list of all roles
        public string Username { get; set; }
        public string OldRole { get; set; }
        public string NewRole { get; set; }
        public ICollection<SelectListItem> Roles { get; set; }
        [Required, Display(Name ="Role")]
        public string Role { get; set; }
    }
}