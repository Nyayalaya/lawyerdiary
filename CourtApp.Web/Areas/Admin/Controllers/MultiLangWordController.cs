using AspNetCoreHero.Results;
using CourtApp.Application.DTOs.Common;
using CourtApp.Application.Features.Common;
using CourtApp.Application.Features.FormBuilder;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
            var viewModel = _mapper.Map<List<MultiLangDictViewModel>>(result.Data);
            return View(viewModel);  
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] List<MultiLangDictViewModel> request)
        {
            if (request == null || !request.Any())
                return BadRequest("No data received");
            var data = _mapper.Map<List<MultiLangDictDto>>(request);
            var command = new UpdateMultilangCommand
            {
                MultiLangDictDtos = data
            };

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(new { message = "Translations saved successfully" });
        }
    }
}
