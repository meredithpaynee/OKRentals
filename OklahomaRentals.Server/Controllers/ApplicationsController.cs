using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OklahomaRentals.Server.API.Requests;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IApplicationEmailService _applicationEmailService;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(
            IApplicationService applicationService,
            IApplicationEmailService applicationEmailService,
            ILogger<ApplicationsController> logger)
        {
            _applicationService = applicationService;
            _applicationEmailService = applicationEmailService;
            _logger = logger;
        }

        // Public "Apply to rent" form: emails the application to the office.
        [HttpPost("submit")]
        [EnableRateLimiting("applications")]
        public async Task<IActionResult> Submit([FromBody] RentalApplicationRequest application)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Honeypot filled in: almost certainly a bot. Pretend it worked.
            if (!string.IsNullOrEmpty(application.Website))
                return Accepted();

            try
            {
                await _applicationEmailService.SendAsync(application);
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to email rental application");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { message = "We couldn't send your application right now." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var applications = await _applicationService.GetAllAsync();
            return Ok(applications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var application = await _applicationService.GetByIdAsync(id);

            if (application == null)
                return NotFound();

            return Ok(application);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApplicationResponse application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdApplication = await _applicationService.CreateAsync(application);

            return CreatedAtAction(nameof(GetById), new { id = createdApplication.Id }, createdApplication);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ApplicationResponse application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _applicationService.UpdateAsync(id, application);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _applicationService.DeleteAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}