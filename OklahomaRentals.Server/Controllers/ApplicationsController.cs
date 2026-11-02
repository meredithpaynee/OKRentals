using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OklahomaRentals.Server.API.Requests;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Controllers
{
    // Applications hold applicants' personal details, so everything except the
    // public submit endpoint requires a signed-in user.
    [Authorize]
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

        // Public "Apply to rent" form: saves the application, then emails it to the office.
        [AllowAnonymous]
        [HttpPost("submit")]
        [EnableRateLimiting("applications")]
        public async Task<IActionResult> Submit([FromBody] RentalApplicationRequest application)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // Honeypot filled in: almost certainly a bot. Pretend it worked.
            if (!string.IsNullOrEmpty(application.Website))
                return Accepted();

            ApplicationResponse? saved = null;
            try
            {
                saved = await _applicationService.SubmitAsync(application);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(ex.ParamName ?? string.Empty, ex.Message);
                return ValidationProblem(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save rental application");
            }

            var emailed = false;
            try
            {
                await _applicationEmailService.SendAsync(application);
                emailed = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to email rental application {Id}", saved?.Id);
            }

            // Either one is enough for the office to see the application.
            if (saved == null && !emailed)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { message = "We couldn't send your application right now." });
            }
            return Accepted();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ApplicationStatusRequest request)
        {
            try
            {
                var updated = await _applicationService.UpdateStatusAsync(id, request.Status);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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