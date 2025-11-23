using CourtApp.Application.Constants;
using CourtApp.Application.DTOs.Mail;
using CourtApp.Application.Interfaces.Shared;
using CourtApp.Infrastructure.DbContexts;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Admin.Models;
using CourtApp.Web.Extensions;
using CourtApp.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : BaseController<UserController>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityContext _identityDbContext;
        private readonly IMailService _emailSender;
        private readonly BlobService _blobService;
        private readonly IDocumentUploadService _documentUploadService;
        public UserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IdentityContext _identityDbContext,
            IMailService emailSender,
            BlobService _blobService,
            IDocumentUploadService documentUploadService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            this._identityDbContext = _identityDbContext;
            _emailSender = emailSender;
            this._blobService = _blobService;
            _documentUploadService = documentUploadService;
        }


        public IActionResult Index()
        {

            return View();
        }

        private async Task<List<UserViewModel>> GetAllUsers()
        {
            var model = new List<UserViewModel>();
            if (User.IsInRole("SuperAdmin"))
            {
                List<string> superAdminUsers = new List<string>() { "LAWYER", "CORPORATE" };
                var superAdminUsersData = await _userManager.Users
                    .Where(a => a.Id != CurrentUser.Id && superAdminUsers
                    .Contains(a.UserType.ToUpper())).Select(user => new UserViewModel
                    {
                        Role = user.UserType,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Mobile = user.Mobile,
                        EmailConfirmed = user.EmailConfirmed,
                        ProfileImgPath = "",
                        ProfilePicture = user.ProfilePicture,
                        Id = user.Id,
                        IsActive = user.IsActive
                    })
                    .ToListAsync();
                model = _mapper.Map<List<UserViewModel>>(superAdminUsersData);
            }
            else
            {
                var otherLoggedUsers = new HashSet<string> { "ASSOCIATE", "CLERK" };
                var operatorIds = await _identityDbContext.LawyerUsers
                    .Where(o => o.LawyerId == CurrentUser.Id)
                    .Select(o => new { o.Id, o.DateOfJoining })
                    .ToListAsync();
                var operatorIdSet = new HashSet<string>(operatorIds.Select(o => o.Id.ToString()));
                model = await _userManager.Users
                    .Where(user => user.Id != CurrentUser.Id
                        && otherLoggedUsers.Contains(user.UserType)
                        && operatorIdSet.Contains(user.Id))
                    .Select(user => new UserViewModel
                    {
                        Role = user.UserType,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Mobile = user.Mobile,
                        EmailConfirmed = user.EmailConfirmed,
                        ProfileImgPath = "",
                        ProfilePicture = user.ProfilePicture,
                        Id = user.Id,
                        IsActive = user.IsActive
                    })
                    .ToListAsync();
            }
            return model;
        }

        public async Task<IActionResult> LoadAll()
        {
            var model = await GetAllUsers();
            _logger.LogInformation("User data is loaded successfully!");
            return PartialView("_ViewAll", model);
        }

        public async Task<IActionResult> OnGetCreate(Guid id)
        {
            var model = new UserViewModel();
            if (id != Guid.Empty)
            {
                var oprUser = await _userManager.Users
                    .Where(a => a.Id == id.ToString())
                    .FirstOrDefaultAsync();
                if (oprUser == null)
                {
                    throw new Exception("Operator user not found.");
                }
                model = _mapper.Map<UserViewModel>(oprUser);
                model.Address = oprUser.AddressInfo?.StreetAddress;
                model.EnrollmentNo = oprUser.ProfessionalInfo?.EnrollmentNo;
                var usrRole = await _userManager.GetRolesAsync(oprUser);
                model.Role = usrRole.FirstOrDefault().ToUpper();
                ViewBag.BtnText = "Update";
            }
            else ViewBag.BtnText = "Save";

            model.Genders = new SelectList(StaticDropDownDictionaries.Gender(), "Key", "Value");
            var lawyerUserRoles = new List<string> { "ASSOCIATE", "CLERK" };
            var roles = _roleManager.Roles
                .Where(w => lawyerUserRoles.Contains(w.NormalizedName))
                .Select(s => new { s.NormalizedName, s.Name }).ToList().OrderBy(o => o.Name);
            model.Roles = new SelectList(roles, "NormalizedName", "Name");
            _logger.LogInformation("User form is loaded successfully!");
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Create", model) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreate(UserViewModel uModel, IFormFile ProfileImgFile)
        {
            if (!ModelState.IsValid)
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_Create", uModel);
                return new JsonResult(new { isValid = false, html = html });
            }

            var lawyer = await _userManager.GetUserAsync(User);
            if (lawyer == null) return Unauthorized();

            ApplicationUser user;
            bool isNewUser = string.IsNullOrEmpty(uModel.Id); // Check if it's a new user

            if (isNewUser)
            {
                // **Check if Email Already Exists**
                var existingUser = await _userManager.FindByEmailAsync(uModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "An account with this email already exists");
                    return new JsonResult(new { isValid = false, html = await _viewRenderer.RenderViewToStringAsync("_Create", new UserViewModel()) });
                }

                // **Create a New User**
                MailAddress address = new MailAddress(uModel.Email);
                string userName = address.User;
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = uModel.Email,
                    FirstName = uModel.FirstName,
                    LastName = uModel.LastName,
                    Mobile = uModel.Mobile,
                    Gender = uModel.Gender,
                    DateOfBirth = uModel.DateOfBirth ?? default(DateTime),
                    UserType = uModel.Role,
                    IsActive = true,
                    ProfessionalInfo = new ProfessionalInfo { EnrollmentNo = uModel.EnrollmentNo },
                    AddressInfo = new AddressInfo { StreetAddress = uModel.Address },
                };

                var result = await _userManager.CreateAsync(user, "123Pa$$word!");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors) _notify.Error(error.Description);
                    return new JsonResult(new { isValid = false, html = await _viewRenderer.RenderViewToStringAsync("_Create", uModel) });
                }

                await _userManager.AddToRoleAsync(user, uModel.Role);
            }
            else
            {
                // **Update Existing User**
                user = await _userManager.FindByIdAsync(uModel.Id);
                if (user == null) return NotFound("User not found.");

                user.FirstName = uModel.FirstName;
                user.LastName = uModel.LastName;
                user.Mobile = uModel.Mobile;
                user.Gender = uModel.Gender;
                user.DateOfBirth = uModel.DateOfBirth ?? default(DateTime);
                user.UserType = uModel.Role;
                user.ProfessionalInfo = new ProfessionalInfo { EnrollmentNo = uModel.EnrollmentNo };
                user.AddressInfo = new AddressInfo { StreetAddress = uModel.Address };
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors) _notify.Error(error.Description);
                    return new JsonResult(new { isValid = false, html = await _viewRenderer.RenderViewToStringAsync("_Create", uModel) });
                }
            }

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                if (file != null)
                {
                    user.ProfilePicture = file.OptimizeImageSize(720, 720);
                    await _userManager.UpdateAsync(user);
                }
            }

            // **Handle Profile Image Upload**
            //if (ProfileImgFile != null)
            //{
            //string fileName = user.Id + System.IO.Path.GetExtension(ProfileImgFile.FileName);
            //using var stream = ProfileImgFile.OpenReadStream();
            //var path = await _documentUploadService.UploadFileAsync(stream, fileName, "Profile");
            //user.ProfileImgPath = path;
            //user.ProfileImgPath = await _blobService.UploadOrUpdateFileAsync(stream, fileName, ProfileImgFile.ContentType, "ProfileImage", cancellationToken: System.Threading.CancellationToken.None);
            //user.ProfilePicture=
            //await _userManager.UpdateAsync(user);
            // }

            // **Handle LawyerUser Data**
            var lawyerUser = await _identityDbContext.LawyerUsers.FindAsync(Guid.Parse(user.Id));
            if (lawyerUser == null)
            {
                lawyerUser = new OperatorUser
                {
                    Id = Guid.Parse(user.Id),
                    LawyerId = lawyer.Id,
                    DateOfJoining = uModel.DateOfJoining ?? default(DateTime),
                    Enrollment = uModel.EnrollmentNo,
                    AddressInfo = new AddressInfo
                    {
                        StateId = 0,
                        CityId = 0,
                        DistrictId = 0,
                        StreetAddress = uModel.Address
                    },
                    CreatedBy = user.Id,
                    CreatedOn = DateTime.Now
                };
                _identityDbContext.LawyerUsers.Add(lawyerUser);
            }
            else
            {
                lawyerUser.DateOfJoining = uModel.DateOfJoining ?? default(DateTime);
                lawyerUser.Enrollment = uModel.EnrollmentNo;
                lawyerUser.AddressInfo.StreetAddress = uModel.Address;
                _identityDbContext.LawyerUsers.Update(lawyerUser);
            }

            await _identityDbContext.SaveChangesAsync();

            // **Send Email Only for New Users**
            if (isNewUser)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                string returnUrl = Url.Content("~/");
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                var mailRequest = new MailRequest
                {
                    Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                    From = "info@sparo.com",
                    To = uModel.Email,
                    Subject = "Please verify your email address\r\n"
                };
                await _emailSender.SendAsync(mailRequest);
            }

            _notify.Success(isNewUser ? "User created successfully!" : "User updated successfully!");
            var model = await GetAllUsers();
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", model) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await DeleteUserAsync(id);
            if (success)
            {
                var model = await GetAllUsers();
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", model) });
            }
            else
            {
                _notify.Error("User is unable to delete");
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                // Ensure user exists
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                // Start a transaction
                using var transaction = await _identityDbContext.Database.BeginTransactionAsync();

                // Remove related Identity records
                var logins = _identityDbContext.UserLogins.Where(l => l.UserId == userId);
                _identityDbContext.UserLogins.RemoveRange(logins);

                var roles = _identityDbContext.UserRoles.Where(r => r.UserId == userId);
                _identityDbContext.UserRoles.RemoveRange(roles);

                var claims = _identityDbContext.UserClaims.Where(c => c.UserId == userId);
                _identityDbContext.UserClaims.RemoveRange(claims);

                var tokens = _identityDbContext.UserTokens.Where(t => t.UserId == userId);
                _identityDbContext.UserTokens.RemoveRange(tokens);

                // Ensure `userId` is a valid GUID before parsing
                if (Guid.TryParse(userId, out Guid parsedUserId))
                {
                    var lawyerUser = _identityDbContext.LawyerUsers.Where(t => t.Id == parsedUserId);
                    _identityDbContext.LawyerUsers.RemoveRange(lawyerUser);
                }

                await _identityDbContext.SaveChangesAsync(); // Commit changes before deleting user

                // Sign out user if they are logged in
                await _signInManager.SignOutAsync();

                await transaction.CommitAsync(); // Commit transaction

                // Delete user from Identity
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded) throw new Exception("User deletion failed");

                // Delete associated files from Azure Blob Storage
                //if (!string.IsNullOrEmpty(user.ProfileImgPath))
                //{
                //    await _documentUploadService.DeleteFileAsync(user.ProfileImgPath);
                //    //await _blobService.DeleteFileAsync(user.ProfileImgPath);
                //}


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

    }
}