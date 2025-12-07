using AspNetCoreHero.Results;
using CourtApp.Application.Features.Common;
using CourtApp.Application.Features.FormBuilder;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MultiLangWordController : BaseController<MultiLangWordController>
    {
        public async Task<IActionResult> IndexAsync(string langCode)
        {
            var result = await _mediator.Send(new GetMultiLangDictQuery() { LangCode = langCode });
            if (!result.Succeeded) return BadRequest(result.Message);

            if (!result.Succeeded)
                return BadRequest(result.Message);

            // Map DTO -> ViewModel
            var viewModel = _mapper.Map<List<MultiLangDictViewModel>>(result.Data);

            return View(viewModel);  
        }
    }
}
