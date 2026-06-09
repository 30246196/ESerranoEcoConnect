using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESerranoEcoConnect.Models.ViewModels
{
    // ViewModel used to present contact form messages in the admin interface.
    // This avoids exposing the full domain model and allows us to include
    // a user-friendly display name instead of a raw UserId.
    public class ContactFormViewModel
    {
        public int ContactFormId { get; set; }

        // The name shown in the table (Member, Staff, or Anonymous).
        public string UserDisplayName { get; set; }

        // The message submitted by the user.
        public string Message { get; set; }

        // The date and time when the message was submitted.
        public DateTime SentAt { get; set; }
    }

}