using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;// added for the [Required] attribute
using System.IO;// added for the Path class?
using System.Linq;
using System.Text;
using System.Web;//added for HttpPostedFileBase


namespace ESerranoEcoConnect.Models
{
    public class Member : User
    {
        ////[Display(Name = "Member Type")]
        ////public MemberType MemberType { get; set; }

        // NAVIGATION PROPERTY: one member can write many comments
        public virtual ICollection<Comment> Comments { get; set; }
    }

    //
    //public enum MemberType
    //{
    //    [Display(Name = "Individual")]
    //    Individual,
    //    [Display(Name = "Organization")]
    //    Organization
    //}
}