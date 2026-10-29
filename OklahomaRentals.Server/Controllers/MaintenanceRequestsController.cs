using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.API.Requests;

namespace OklahomaRentals.Server.Controllers
{
    [ApiController]
    [Route("api/maintenance-requests")]
    public class MaintenanceRequestsController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceRequestsController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _maintenanceService.GetAllAsync();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _maintenanceService.GetByIdAsync(id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MaintenanceRequestCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdRequest = await _maintenanceService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = createdRequest.Id }, createdRequest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _maintenanceService.DeleteAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] MaintenanceRequestUpdateStatusRequest request)
        {
            try
            {
                var updated = await _maintenanceService.UpdateStatusAsync(id, request.Status);

                if (!updated)
                    return NotFound();

                return Ok("Status updated");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}