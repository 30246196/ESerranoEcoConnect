using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using Microsoft.Analytics.Interfaces;
//using Microsoft.Analytics.Types.Sql;
using System.ComponentModel.DataAnnotations;// added for the [Required] attribute
using System.Web;//added for HttpPostedFileBase

namespace ESerranoEcoConnect.Models
{
    public class Staff : User
    {
        [Display(Name = "Staff Role")]
        public StaffRole StaffRole { get; set; }
    }

    public enum StaffRole // should be IdentityRole?
    {
        [Display(Name = "Admin")]
        Admin,
        [Display(Name = "Moderator")]
        Moderator
    }
}