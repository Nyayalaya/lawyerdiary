using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Account;
using CourtApp.Application.Features.Account;
using CourtApp.Infrastructure.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public partial class BillingDetailModel : PageModel
{
    private readonly ILogger<BillingDetailModel> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private IMediator _mediatorInstance;

    public BillingDetailModel(
        ILogger<BillingDetailModel> logger,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    // Lazy loading mediator instance from DI
    protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();

    [BindProperty]
    public BillingDetailInputModel Input { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public class BillingDetailInputModel
    {
        [Required, Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Required, Display(Name = "Account Number")]
        public string AccountNo { get; set; }

        [Required, Display(Name = "Branch Name")]
        public string Branch { get; set; }

        [Required, Display(Name = "IFSC Code")]
        public string IfscCode { get; set; }

        [Display(Name = "Pan Number")]
        public string PanNumber { get; set; }

        [Display(Name = "GST No")]
        public string GstNo { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");

        var result = await _mediator.Send(new BillingDetailGetByLawyerQuery { LawyerId = user.Id });

        if (!result.Succeeded || result.Data == null)
            return Page(); // Render empty form if no billing detail yet

        var billing = result.Data;
        Input = new BillingDetailInputModel
        {
            AccountNo = billing.AccountNo,
            BankName = billing.BankName,
            Branch = billing.Branch,
            GstNo = billing.GstNo,
            IfscCode = billing.IfscCode,
            PanNumber = billing.PanNumber
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");

        if (!ModelState.IsValid)
            return Page();

        // Check if a billing detail already exists
        var existingResult = await _mediator.Send(new BillingDetailGetByLawyerQuery
        {
            LawyerId = user.Id
        });

        IResult result;

        var dto = new BillingDetailDto
        {
            LawyerId = user.Id,
            AccountNo = Input.AccountNo?.Trim(),
            BankName = Input.BankName?.Trim(),
            Branch = Input.Branch?.Trim(),
            GstNo = Input.GstNo?.Trim(),
            IfscCode = Input.IfscCode?.Trim(),
            PanNumber = Input.PanNumber?.Trim()
        };

        if (existingResult.Succeeded && existingResult.Data != null)
        {
            dto.Id = existingResult.Data.Id;

            result = await _mediator.Send(new BillingDetailUpdateCommand
            {
                BillingDetail = dto
            });
        }
        else
        {
            result = await _mediator.Send(new BillingDetailCreateCommand
            {
                BillingDetail = dto
            });
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Failed to save billing detail.");
            return Page();
        }

        TempData["SuccessMessage"] = "Billing detail saved successfully.";
        return RedirectToPage();
    }
}
