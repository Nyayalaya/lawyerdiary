using CourtApp.Application.Constants;
using CourtApp.Application.DTOs.Mail;
using CourtApp.Application.Interfaces.Repositories;
using CourtApp.Application.Interfaces.Shared;
using CourtApp.Infrastructure.DbContexts;
using CourtApp.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMailService _emailSender;
        private readonly IdentityContext _identityDbContext;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IMailService emailSender,
            ILawyerRepository lawyerRepository,
            IUnitOfWork uow, IdentityContext _identityDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            this._identityDbContext = _identityDbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        //[BindProperty]
        //public RegisterViewModel RegData { get; set; } = new();
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public SelectList Genders { get; set; }

        public class InputModel
        {
            [Required]
            public string UserType { get; set; }

            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            public string EnrollmentNo { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }


            [Required]
            [Display(Name = "Mobile No")]
            public string Mobile { get; set; }

            [Display(Name = "Website")]
            public string Website { get; set; }

            [Display(Name = "Telphone")]
            public string Telephone { get; set; }

            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Genders = new SelectList(StaticDropDownDictionaries.Gender(), "Key", "Value");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                string emailId = string.Empty;
                ApplicationUser user = new ApplicationUser();
                MailAddress address = new MailAddress(Input.Email);
                string userName = address.User;
                string[] parts = Input.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string lastName= parts.Length > 0 ? parts[^1] : string.Empty;
                string fsName= parts.Length > 0 ? parts[0] : string.Empty;
                user = new ApplicationUser
                {
                    UserType = Input.UserType.ToUpper(),
                    UserName = userName.ToUpper(),
                    Email = Input.Email,
                    FirstName = fsName.ToUpper(),
                    LastName= lastName.ToUpper(),
                    Mobile = Input.Mobile,                    
                    ProfessionalInfo = Input.UserType.ToUpper() == "LAWYER" ? new ProfessionalInfo
                    {
                        EnrollmentNo = Input.EnrollmentNo                        
                    } : null                    
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, Input.UserType.ToUpper().ToString());
                    if (Input.UserType.ToUpper() == "Corporate".ToUpper())
                    {
                        CorporateUser cur = new CorporateUser();
                        cur.FirmName = Input.Name;
                        cur.Id = user.Id;
                        cur.RegistrationNo = Input.EnrollmentNo;
                        _identityDbContext.Corporates.Add(cur);
                        await _identityDbContext.SaveChangesAsync();
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new
                        {
                            area = "Identity",
                            userId = user.Id,
                            code = code,
                            returnUrl = returnUrl
                        },
                        protocol: Request.Scheme);
                    var mailRequest = new MailRequest
                    {
                        Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                        To = Input.Email,
                        Subject = "Please verify your email address\r\n"
                    };
                    await _emailSender.SendAsync(mailRequest);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}