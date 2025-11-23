using CourtApp.Infrastructure.DbContexts;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Identity.Pages
{
    public class ProfileModel : PageModel
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public List<string> Roles { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public LaywerViewModel LaywerInfo { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IdentityContext _identityContext;

        public ProfileModel(UserManager<ApplicationUser> userManager, IdentityContext _identityContext)
        {
            this._identityContext = _identityContext;
            _userManager = userManager;
        }

        public async Task OnGetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            //var usrdt = _identityContext.Lawyers
            //    .Where(w => w.Id.Equals(user.Id))
            //    .FirstOrDefault();
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserId = userId;
                Username = user.UserName;
                ProfilePicture = user.ProfilePicture;
                FirstName = user.FirstName;
                LastName = user.LastName;
                IsActive = user.IsActive;
                Mobile = user.Mobile;
                Email = user.Email;
                IsSuperAdmin = roles.Contains("SuperAdmin");
                Roles = roles.ToList();
                Phone = user.PhoneNumber;
                LaywerInfo = new LaywerViewModel()
                {
                    ProfInfo = new ProfessionalInfoViewModel
                    {
                        EnrollmentNo = user != null && user.ProfessionalInfo!=null? user.ProfessionalInfo.EnrollmentNo : "",
                        BarAssociationNumber = user != null && user.ProfessionalInfo != null ? user.ProfessionalInfo.BarAssociationNumber : "",
                        PracticeLicenseDate= user != null && user.ProfessionalInfo != null ? user.ProfessionalInfo.PracticeLicenseDate : System.DateTime.Now,
                        PracticeSince = user != null && user.ProfessionalInfo != null ? user.ProfessionalInfo.PracticeSince.ToString() : "",
                    },
                    AddressInfo = new AddressInfoViewModel()
                    {
                        StreetAddress = user != null&& user.AddressInfo!=null ? user.AddressInfo.StreetAddress : ""
                    },
                    WorkLocInfo = new WorkInfoViewModel()
                    {
                        Address = user != null && user.WorkLocInfo !=null? user.WorkLocInfo.Address : ""
                    }
                };
            }
        }

        public async Task<IActionResult> OnPostActivateUserAsync(string userId)
        {
            if (User.IsInRole("SuperAdmin"))
            {
                var currentUser = await _userManager.FindByIdAsync(userId);
                currentUser.IsActive = true;
                //currentUser.ActivatedBy = _userManager.GetUserAsync(HttpContext.User).Result.Id;
                await _userManager.UpdateAsync(currentUser);
                await OnGetAsync(userId);
                return RedirectToPage("Profile", new { area = "Identity", userId = userId });
            }
            else return default;
        }

        public async Task<IActionResult> OnPostDeActivateUserAsync(string userId)
        {
            if (User.IsInRole("SuperAdmin"))
            {
                var currentUser = await _userManager.FindByIdAsync(userId);
                currentUser.IsActive = false;
                await _userManager.UpdateAsync(currentUser);
                return RedirectToPage("Profile", new { area = "Identity", userId = userId });
            }
            else return default;
        }
    }
}