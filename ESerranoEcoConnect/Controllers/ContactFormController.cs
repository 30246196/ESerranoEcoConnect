using ESerranoEcoConnect.Models;
using ESerranoEcoConnect.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ESerranoEcoConnect.Controllers
{
    // staf members: Admin, Moderator and Staff
    [Authorize(Roles = "Admin,Moderator,Staff")]
    public class ContactFormController : Controller
    {
        private readonly EcoConnectDbContext _context = new EcoConnectDbContext();

        // GET: ContactForm
        public async Task<ActionResult> Index()
        {
            // Retrieves all contact form messages in descending date order.
            // These messages are read‑only and cannot be edited or deleted.
            var messages = await _context.ContactForms
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var model = new List<ContactFormViewModel>();

            foreach (var m in messages)
            {
                // Determine how the user's name should be displayed.
                // If the message was submitted by an anonymous visitor, show a default label.
                // Otherwise, attempt to match the UserId to either a Member or a Staff account.
                string displayName;

                if (string.IsNullOrEmpty(m.UserId))
                {
                    // Message submitted by a visitor who is not logged in.
                    displayName = "Anonymous / Not registered";
                }
                else
                {
                    // Attempt to find the user in the Members table.
                    var member = await _context.Members
                        .FirstOrDefaultAsync(u => u.Id == m.UserId);

                    // If not found, attempt to find the user in the Staff table.
                    var staff = member == null
                        ? await _context.Staffs.FirstOrDefaultAsync(u => u.Id == m.UserId)
                        : null;

                    // Build the display name depending on the user type.
                    if (member != null)
                        displayName = $"{member.FirstName} {member.LastName}";
                    else if (staff != null)
                        displayName = $"{staff.FirstName} {staff.LastName}";
                    else
                        // Fallback in case the UserId no longer exists in the system.
                        displayName = "Unknown user";
                }


                model.Add(new ContactFormViewModel
                {
                    ContactFormId = m.ContactFormId,
                    UserDisplayName = displayName,
                    Message = m.Message,
                    SentAt = m.SentAt
                });
            }

            return View(model);
        }


        // GET: ContactForm/Details/5
                
         public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var message = await _context.ContactForms
                .FirstOrDefaultAsync(m => m.ContactFormId == id);

            if (message == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            // Resolve display name (same logic as Index)
            string displayName;

            if (string.IsNullOrEmpty(message.UserId))
            {
                displayName = "Anonymous / Not registered";
            }
            else
            {
                var member = await _context.Members
                    .FirstOrDefaultAsync(u => u.Id == message.UserId);

                var staff = member == null
                    ? await _context.Staffs.FirstOrDefaultAsync(u => u.Id == message.UserId)
                    : null;

                if (member != null)
                    displayName = $"{member.FirstName} {member.LastName}";
                else if (staff != null)
                    displayName = $"{staff.FirstName} {staff.LastName}";
                else
                    displayName = "Unknown user";
            }

            // Build the ViewModel
            var vm = new ContactFormViewModel
            {
                ContactFormId = message.ContactFormId,
                Message = message.Message,
                SentAt = message.SentAt,
                UserDisplayName = displayName
            };

            return View(vm);   // ✔ ahora sí coincide con la vista
        }


    }
}
