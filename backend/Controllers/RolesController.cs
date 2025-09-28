using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitem_backend.Data;

namespace recruitem_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<RolesController> _logger;

        public RolesController(DatabaseContext context, ILogger<RolesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            _logger.LogInformation("Getting all roles");

            var roles = await _context.Roles
                .Select(r => new { r.Id, r.RoleName })
                .ToListAsync();

            return Ok(roles);
        }
    }
}
