using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.API.Requests;

namespace OklahomaRentals.Server.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/properties")]
    public class PropertiesController : ControllerBase
    {
        // Controller depends on the interface, not a concrete class
        private readonly IPropertyService _propertyService;
        private readonly ICloudinaryService _cloudinaryService;
        // ASP.NET injects the registered service here (DI)
        public PropertiesController(IPropertyService propertyService, ICloudinaryService cloudinaryService)
        {
            _propertyService = propertyService;
            _cloudinaryService = cloudinaryService;
        }

        /*** Temporary endpoint for testing authentication. Remove before production. ***/
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("authenticated");
        }

        // GET /api/properties
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var properties = await _propertyService.GetAllAsync();
            return Ok(properties);
        }

        // GET /api/properties/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var property = await _propertyService.GetByIdAsync(id);

            if (property == null)
                return NotFound(); // 404 if service returns null

            return Ok(property);
        }

        [HttpGet("{folder}/images")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImages(string folder)
        {
            try
            {
                var decodedFolder = Uri.UnescapeDataString(folder);
                var images = await _cloudinaryService.GetImagesByFolderAsync(decodedFolder);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST /api/properties
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PropertyRequest property)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProperty = await _propertyService.CreateAsync(property);

            // Returns a 201 Created status, with a Location header pointing to the new resource
            return CreatedAtAction(nameof(GetById), new { id = createdProperty.Id }, createdProperty);
        }

        // PUT /api/properties/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PropertyRequest property)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _propertyService.UpdateAsync(id, property);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        // PATCH /api/properties/{id}/description
        [Authorize]
        [HttpPatch("{id}/description")]
        public async Task<IActionResult> UpdateDescription(int id, [FromBody] PropertyDescriptionUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _propertyService.UpdateDescriptionAsync(id, request.Description);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        // PATCH /api/properties/{id}/availability
        [AllowAnonymous]
        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] PropertyAvailabilityUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _propertyService.UpdateAvailabilityAsync(id, request.IsLeased);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        // POST /api/properties/filler
        // Helpful endpoint for quick testing without sending a JSON body
        [Authorize]
        [HttpPost("filler")]
        public async Task<IActionResult> CreateFiller()
        {
            var createdProperty = await _propertyService.CreateFillerTempProperty();
            return CreatedAtAction(nameof(GetById), new { id = createdProperty.Id }, createdProperty);
        }

        // DELETE /api/properties/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _propertyService.DeleteAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("byname/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByName(string name)
        {
            var property = await _propertyService.GetByNameAsync(name);

            if (property == null)
                return NotFound();

            return Ok(property);
        }
    }
}