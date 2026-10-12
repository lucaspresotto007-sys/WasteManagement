using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WasteManagementAPI.Services;
using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WasteBinController : ControllerBase
    {
        private readonly IWasteBinService _binService;

        public WasteBinController(IWasteBinService binService)
        {
            _binService = binService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBins(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? materialType = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _binService.GetBinsAsync(pageNumber, pageSize, materialType);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBinById(int id)
        {
            var bin = await _binService.GetBinByIdAsync(id);
            if (bin == null)
            {
                return NotFound(new { message = $"Lixeira com ID {id} não encontrada." });
            }
            return Ok(bin);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Collector")]
        public async Task<IActionResult> CreateBin([FromBody] WasteBinRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bin = await _binService.CreateBinAsync(model);
            return CreatedAtAction(nameof(GetBinById), new { id = bin.Id }, bin);
        }

        [HttpPut("{id}/fill-level")]
        [Authorize(Roles = "Admin,Collector")]
        public async Task<IActionResult> UpdateFillLevel(int id, [FromBody] WasteBinFillLevelUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBin = await _binService.UpdateFillLevelAsync(id, model.CurrentFillLevelKg);
            if (updatedBin == null)
            {
                return NotFound(new { message = $"Lixeira com ID {id} não encontrada." });
            }

            return Ok(updatedBin);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBin(int id)
        {
            var success = await _binService.DeleteBinAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Lixeira com ID {id} não encontrada." });
            }
            return NoContent();
        }
    }
}
