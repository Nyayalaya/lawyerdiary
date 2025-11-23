using CourtApp.Application.Features.CaseDetails;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Litigation.Controllers
{
    [Area("Litigation")]
    public class AssignedCaseController : BaseController<AssignedCaseController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AssignedCaseController(UserManager<ApplicationUser> _userManager)
        {
            this._userManager = _userManager;
        }
        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new LawyerWiseAssignedQuery()
            {
                LinkedIds = User.GetUserLinkedIds(),
            });
            if (response.Succeeded)
            {
                List<string> superAdminUsers = new List<string>() { "LAWYER", "CORPORATE" };

                var lawyerDt = response.Data; // This is List<LawyerWiseAssignedCaseDto>
                var lawyerIds = lawyerDt.Select(s => s.LawyerId.ToString()).ToList();
                var lawyerDetail = await _userManager.Users
                    .Where(a => a.Id != CurrentUser.Id
                                && superAdminUsers.Contains(a.UserType.ToUpper())
                                && lawyerIds.Contains(a.Id)
                          )
                    .Select(user => new
                    {
                        Id = user.Id,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty
                    }).ToListAsync();

                //var fnResult = (from ld in lawyerDt
                //                join ud in lawyerDetail on ld.LawyerId equals ud.Id
                //                select new
                //                {
                //                    LawyerId = ud.Id,
                //                    FirstName = ud.FirstName,
                //                    LastName = ud.LastName,
                //                    AssignedCaseInfo = ld.AssignedCaseInfo
                //                }).ToList();






                //var viewModel = _mapper.Map<List<GetCaseInfoViewModel>>(response.Data);
                //_logger.LogInformation("Load all the user's cases successfully!");
                //return PartialView("_ViewAll", viewModel);
            }
            return null;
        }
    }
}
