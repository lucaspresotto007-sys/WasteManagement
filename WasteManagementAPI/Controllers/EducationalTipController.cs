using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteManagementAPI.Data;
using WasteManagementAPI.Models;
using WasteManagementAPI.ViewModels;

namespace WasteManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationalTipController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EducationalTipController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTips(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? category = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.EducationalTips.AsQueryable();
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(t => t.MaterialCategory.ToLower() == category.ToLower());
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new EducationalTipResponseViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Content = t.Content,
                    MaterialCategory = t.MaterialCategory
                })
                .ToListAsync();

            var result = new PagedResultViewModel<EducationalTipResponseViewModel>(items, pageNumber, pageSize, totalItems);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipById(int id)
        {
            var tip = await _context.EducationalTips.FindAsync(id);
            if (tip == null)
            {
                return NotFound(new { message = $"Dica {id} não encontrada." });
            }

            return Ok(new EducationalTipResponseViewModel
            {
                Id = tip.Id,
                Title = tip.Title,
                Content = tip.Content,
                MaterialCategory = tip.MaterialCategory
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTip([FromBody] EducationalTipRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tip = new EducationalTip
            {
                Title = model.Title,
                Content = model.Content,
                MaterialCategory = model.MaterialCategory
            };

            _context.EducationalTips.Add(tip);
            await _context.SaveChangesAsync();

            var response = new EducationalTipResponseViewModel
            {
                Id = tip.Id,
                Title = tip.Title,
                Content = tip.Content,
                MaterialCategory = tip.MaterialCategory
            };

            return CreatedAtAction(nameof(GetTipById), new { id = tip.Id }, response);
        }
    }
}
