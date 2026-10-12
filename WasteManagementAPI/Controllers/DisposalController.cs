using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasteManagementAPI.Services;
using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DisposalController : ControllerBase
    {
        private readonly IDisposalService _disposalService;

        public DisposalController(IDisposalService disposalService)
        {
            _disposalService = disposalService;
        }

        [HttpPost]
        public async Task<IActionResult> RecordDisposal([FromBody] DisposalRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Identificador de usuário inválido no token." });
            }

            var result = await _disposalService.RecordDisposalAsync(userId, model);
            if (result == null)
            {
                return BadRequest(new { message = "Não foi possível registrar o descarte. Verifique se a lixeira existe." });
            }

            return Ok(result);
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Identificador de usuário inválido no token." });
            }

            var result = await _disposalService.GetUserDisposalsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("all-history")]
        [Authorize(Roles = "Admin,Collector")]
        public async Task<IActionResult> GetAllHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _disposalService.GetAllDisposalsAsync(pageNumber, pageSize);
            return Ok(result);
        }
    }
}
