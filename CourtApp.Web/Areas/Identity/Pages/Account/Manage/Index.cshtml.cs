using AuditTrail.Abstrations;
using CourtApp.Domain.Entities.LawyerDiary;
using CourtApp.Infrastructure.DbContexts;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Web.Extensions;
using CourtApp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IdentityContext _identityContext;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IdentityContext _identityContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._identityContext = _identityContext;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public SelectList Specializations { get; set; }
        public SelectList Years { get; set; }
        public SelectList States { get; set; }
        public SelectList CourtTypes { get; set; }
        public SelectList CourtDistricts { get; set; }
        public SelectList Complexes { get; set; }
        public SelectList Courts { get; set; }
        public SelectList Genders { get; set; }

        public class InputModel
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Username")]
            public string Username { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile Picture")]
            public byte[] ProfilePicture { get; set; }
            public string Gender { get; set; }
            public string Mobile { get; set; }

            [Display(Name = "Date of birth")]
            public DateTime DateOfBirth { get; set; }

            [Display(Name = "Date of joining")]
            public DateTime DateOfJoining { get; set; }

            public ProfessionalInfo ProfInfo { get; set; }
            public WorkLocation WorkLocInfo { get; set; }
            public AddressInfo AddressInfo { get; set; }

        }

        public class AddressInfo
        {
            public int StateId { get; set; }
            public int DistrictId { get; set; }
            public int CityId { get; set; }
            [Display(Name = "Complete Address")]
            public string Address { get; set; }
            public string Landmark { get; set; }
        }

        public class WorkLocation
        {
            public string Address { get; set; }
            public int StateId { get; set; }
            public int DistrictId { get; set; }
            public Guid CourtTypeId { get; set; }
            public Guid CourtId { get; set; }
        }

        public class ContactInfo
        {
            public string O_Phone { get; set; }
            public string R_Phone { get; set; }
            public string P_Email { get; set; }
            public string O_Email { get; set; }
        }

        public class ProfessionalInfo
        {
            [Display(Name = "Enrollment Number")]
            public string EnrollmentNo { get; set; }

            [Display(Name = "Bar Association Number")]
            public string BarAssociationNumber { get; set; }

            [Display(Name = "License Date")]
            public DateTime PracticeLicenseDate { get; set; }

            //[Display(Name = "Practice Since")]
            //public int PracticeSince { get; set; }

            //[Display(Name = "Specialization Date")]
            //public Guid SpecializationId { get; set; }

        }


        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user?.FirstName;
            var lastName = user?.LastName;
            var profilePicture = user?.ProfilePicture;
            var addressInfo = user?.AddressInfo;
            var workLocInfo = user?.WorkLocInfo;
            var contactInfo = user?.ContactInfo;
            var profInfo = user?.ProfessionalInfo;

            Username = userName;
            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Username = userName,
                FirstName = firstName,
                LastName = lastName,
                Mobile = user?.Mobile,
                Gender = user?.Gender,
                DateOfBirth = (DateTime)(user?.DateOfBirth),
                DateOfJoining = DateTime.Now,
                ProfilePicture = profilePicture,
                AddressInfo = new AddressInfo
                {
                    Address = addressInfo?.StreetAddress,
                    CityId = addressInfo?.CityId ?? 0,
                    DistrictId = addressInfo?.DistrictId ?? 0,
                    StateId = addressInfo?.StateId ?? 0
                },
                WorkLocInfo = new WorkLocation
                {
                    DistrictId = workLocInfo?.DistrictId ?? 0,
                    CourtId = workLocInfo?.CourtId ?? Guid.Empty,
                    CourtTypeId = workLocInfo?.CourtTypeId ?? Guid.Empty,
                    StateId = workLocInfo?.StateId ?? 0,
                    Address = workLocInfo?.Address??""
                },
                ProfInfo = new ProfessionalInfo
                {
                    BarAssociationNumber = profInfo?.BarAssociationNumber ?? string.Empty,
                    EnrollmentNo = profInfo?.EnrollmentNo ?? string.Empty,
                    PracticeLicenseDate = profInfo?.PracticeLicenseDate ?? DateTime.MinValue,
                    //PracticeSince = profInfo?.PracticeSince ?? 0,
                    //SpecializationId = userProfileData?.ProfessionalInfo?.SpecializationId ?? 0,
                }
            };
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            bool isUserUpdated = false;

            // Phone number update
            var currentPhoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != currentPhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Check and update user properties
            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
                isUserUpdated = true;
            }

            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
                isUserUpdated = true;
            }
            if (Input.Mobile != user.Mobile)
            {
                user.Mobile = Input.Mobile;
                isUserUpdated = true;
            }
            if (Input.DateOfBirth != user.DateOfBirth)
            {
                user.DateOfBirth = Input.DateOfBirth;
                isUserUpdated = true;
            }

            // Address Info
            var address = user.AddressInfo ?? new Infrastructure.Identity.Models.AddressInfo();
            if (address.StreetAddress != Input.AddressInfo.Address ||
                address.StateId != Input.AddressInfo.StateId ||
                address.CityId != Input.AddressInfo.CityId ||
                address.DistrictId != Input.AddressInfo.DistrictId)
            {
                user.AddressInfo = new Infrastructure.Identity.Models.AddressInfo
                {
                    StreetAddress = Input.AddressInfo.Address,
                    StateId = Input.AddressInfo.StateId,
                    CityId = Input.AddressInfo.CityId,
                    DistrictId = Input.AddressInfo.DistrictId
                };
                isUserUpdated = true;
            }

            // Work Location Info
            var workLoc = user.WorkLocInfo ?? new Infrastructure.Identity.Models.WorkLocation();
            if (workLoc.StateId != Input.WorkLocInfo.StateId ||
                workLoc.DistrictId != Input.WorkLocInfo.DistrictId ||
                workLoc.CourtId != Input.WorkLocInfo.CourtId ||
                workLoc.CourtTypeId != Input.WorkLocInfo.CourtTypeId||
                workLoc.Address!=Input.WorkLocInfo.Address)
            {
                user.WorkLocInfo = new Infrastructure.Identity.Models.WorkLocation
                {
                    StateId = Input.WorkLocInfo.StateId,
                    DistrictId = Input.WorkLocInfo.DistrictId,
                    CourtId = Input.WorkLocInfo.CourtId,
                    CourtTypeId = Input.WorkLocInfo.CourtTypeId,
                    Address=Input.WorkLocInfo.Address
                };
                isUserUpdated = true;
            }

            // Professional Info
            var prof = user.ProfessionalInfo ?? new Infrastructure.Identity.Models.ProfessionalInfo();
            if (prof.BarAssociationNumber != Input.ProfInfo.BarAssociationNumber ||
                prof.EnrollmentNo != Input.ProfInfo.EnrollmentNo ||
                prof.PracticeLicenseDate != Input.ProfInfo.PracticeLicenseDate //||
               /* prof.PracticeSince != Input.ProfInfo.PracticeSince*/)
            {
                user.ProfessionalInfo = new Infrastructure.Identity.Models.ProfessionalInfo
                {
                    BarAssociationNumber = Input.ProfInfo.BarAssociationNumber,
                    EnrollmentNo = Input.ProfInfo.EnrollmentNo,
                    PracticeLicenseDate = Input.ProfInfo.PracticeLicenseDate,
                    //PracticeSince = Input.ProfInfo.PracticeSince
                };
                isUserUpdated = true;
            }

            // Handle profile picture upload
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                if (file != null)
                {
                    user.ProfilePicture = file.OptimizeImageSize(720, 720);
                    isUserUpdated = true;
                }
            }

            // Save changes if any
            if (isUserUpdated)
            {
                await _userManager.UpdateAsync(user);
                // Update claims
                await UpdateClaimsAsync(user);

            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private async Task UpdateClaimsAsync(ApplicationUser user)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);

            // Helper method to update or add claim
            async Task SetClaim(string type, string value)
            {
                var oldClaim = existingClaims.FirstOrDefault(c => c.Type == type);

                if (oldClaim == null)
                {
                    // Add new claim if missing
                    await _userManager.AddClaimAsync(user, new Claim(type, value ?? ""));
                }
                else if (oldClaim.Value != value)
                {
                    // Replace claim value if changed
                    await _userManager.ReplaceClaimAsync(user, oldClaim, new Claim(type, value ?? ""));
                }
            }

            // === Update your claims here ===
            await SetClaim(AppClaimType.GivenName, user.FirstName);
            await SetClaim(AppClaimType.Surname, user.LastName);
            await SetClaim(AppClaimType.MobilePhone, user.Mobile);
            await SetClaim(AppClaimType.StreetAddress, user.AddressInfo?.StreetAddress);
            await SetClaim(AppClaimType.EnrollmentNo, user.ProfessionalInfo?.EnrollmentNo);
            await SetClaim(AppClaimType.Email, user.Email);
        }

    }
}