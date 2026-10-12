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
    public class CollectionRouteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CollectionRouteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.CollectionRoutes.OrderByDescending(r => r.ScheduledDate);
            var totalItems = await query.CountAsync();
            
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new CollectionRouteResponseViewModel
                {
                    Id = r.Id,
                    RouteName = r.RouteName,
                    DriverName = r.DriverName,
                    ScheduledDate = r.ScheduledDate,
                    Status = r.Status
                })
                .ToListAsync();

            var result = new PagedResultViewModel<CollectionRouteResponseViewModel>(items, pageNumber, pageSize, totalItems);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRouteById(int id)
        {
            var route = await _context.CollectionRoutes.FindAsync(id);
            if (route == null)
            {
                return NotFound(new { message = $"Rota {id} não encontrada." });
            }

            return Ok(new CollectionRouteResponseViewModel
            {
                Id = route.Id,
                RouteName = route.RouteName,
                DriverName = route.DriverName,
                ScheduledDate = route.ScheduledDate,
                Status = route.Status
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Collector")]
        public async Task<IActionResult> CreateRoute([FromBody] CollectionRouteRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var route = new CollectionRoute
            {
                RouteName = model.RouteName,
                DriverName = model.DriverName,
                ScheduledDate = model.ScheduledDate,
                Status = "Pending"
            };

            _context.CollectionRoutes.Add(route);
            await _context.SaveChangesAsync();

            var response = new CollectionRouteResponseViewModel
            {
                Id = route.Id,
                RouteName = route.RouteName,
                DriverName = route.DriverName,
                ScheduledDate = route.ScheduledDate,
                Status = route.Status
            };

            return CreatedAtAction(nameof(GetRouteById), new { id = route.Id }, response);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Collector")]
        public async Task<IActionResult> UpdateRouteStatus(int id, [FromBody] CollectionRouteStatusUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var route = await _context.CollectionRoutes.FindAsync(id);
            if (route == null)
            {
                return NotFound(new { message = $"Rota {id} não encontrada." });
            }

            route.Status = model.Status;
            _context.CollectionRoutes.Update(route);
            await _context.SaveChangesAsync();

            return Ok(new CollectionRouteResponseViewModel
            {
                Id = route.Id,
                RouteName = route.RouteName,
                DriverName = route.DriverName,
                ScheduledDate = route.ScheduledDate,
                Status = route.Status
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _context.CollectionRoutes.FindAsync(id);
            if (route == null)
            {
                return NotFound(new { message = $"Rota {id} não encontrada." });
            }

            _context.CollectionRoutes.Remove(route);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
