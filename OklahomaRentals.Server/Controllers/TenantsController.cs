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
    [Route("api/tenants")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IPaymentService _paymentService;
        private readonly IMaintenanceService _maintenanceService;

        public TenantsController(
            ITenantService tenantService,
            IPaymentService paymentService,
            IMaintenanceService maintenanceService)
        {
            _tenantService = tenantService;
            _paymentService = paymentService;
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenants = await _tenantService.GetAllAsync();
            return Ok(tenants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tenant = await _tenantService.GetByIdAsync(id);

            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        // GET /api/tenants/property/{propertyId}
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetTenantsByProperty(int propertyId)
        {
            var tenants = await _tenantService.GetTenantsByPropertyAsync(propertyId);
            return Ok(tenants);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TenantResponse tenant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdTenant = await _tenantService.CreateAsync(tenant);
                return CreatedAtAction(nameof(GetById), new { id = createdTenant.Id }, createdTenant);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tenant creation error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = "Failed to create tenant", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TenantResponse tenant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _tenantService.UpdateAsync(id, tenant);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        // PATCH /api/tenants/{tenantId}/assign-property
        [HttpPatch("{tenantId}/assign-property")]
        public async Task<IActionResult> AssignTenantToProperty(int tenantId, [FromBody] AssignTenantRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _tenantService.AssignTenantToPropertyAsync(tenantId, request.PropertyId);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        // GET /api/tenants/{tenantId}/payment-history
        [HttpGet("{tenantId}/payment-history")]
        public async Task<IActionResult> GetPaymentHistory(int tenantId)
        {
            var payments = await _paymentService.GetPaymentsByTenantAsync(tenantId);
            return Ok(payments);
        }

        // GET /api/tenants/{tenantId}/maintenance-history
        [HttpGet("{tenantId}/maintenance-history")]
        public async Task<IActionResult> GetMaintenanceHistory(int tenantId)
        {
            var requests = await _maintenanceService.GetMaintenanceRequestsByTenantAsync(tenantId);
            return Ok(requests);
        }

        // GET /api/tenants/{tenantId}/lease-information
        [HttpGet("{tenantId}/lease-information")]
        public async Task<IActionResult> GetLeaseInformation(int tenantId)
        {
            var leaseInfo = await _tenantService.GetLeaseInformationAsync(tenantId);
            if (leaseInfo == null)
                return NotFound();

            return Ok(leaseInfo);
        }

        // GET /api/tenants/{tenantId}/account-summary
        [HttpGet("{tenantId}/account-summary")]
        public async Task<IActionResult> GetAccountSummary(int tenantId)
        {
            var summary = await _tenantService.GetAccountSummaryAsync(tenantId);
            if (summary == null)
                return NotFound();

            return Ok(summary);
        }

        // GET /api/tenants/{tenantId}/upcoming-payments
        [HttpGet("{tenantId}/upcoming-payments")]
        public async Task<IActionResult> GetUpcomingPayments(int tenantId)
        {
            var upcomingPayments = await _tenantService.GetUpcomingPaymentsAsync(tenantId);
            return Ok(upcomingPayments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tenantService.DeleteAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}