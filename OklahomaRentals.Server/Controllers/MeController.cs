using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OklahomaRentals.Server.API.Requests;
using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Auth;
using OklahomaRentals.Server.Services.Interfaces;

namespace OklahomaRentals.Server.Controllers
{
    // The signed-in user's own data. A login is linked to a tenant record when
    // its verified Keycloak email matches the tenant's email; these endpoints
    // never take a tenant id, so one tenant can't read another's records.
    [ApiController]
    [Route("api/me")]
    [Authorize(Policy = AuthPolicies.SignedIn)]
    public class MeController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IPropertyService _propertyService;
        private readonly IPaymentService _paymentService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly ILogger<MeController> _logger;

        public MeController(
            ITenantService tenantService,
            IPropertyService propertyService,
            IPaymentService paymentService,
            IMaintenanceService maintenanceService,
            ILogger<MeController> logger)
        {
            _tenantService = tenantService;
            _propertyService = propertyService;
            _paymentService = paymentService;
            _maintenanceService = maintenanceService;
            _logger = logger;
        }

        // Who's signed in, whether they're staff, and their lease if they have one.
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tenant = await FindTenantAsync();
            var property = tenant != null ? await _propertyService.GetByIdAsync(tenant.PropertyId) : null;

            return Ok(new
            {
                isAdmin = KeycloakClaims.IsAdmin(User),
                tenant = tenant == null ? null : new
                {
                    tenant.Id,
                    tenant.Name,
                    tenant.Email,
                    tenant.PropertyId,
                    PropertyName = property?.Name,
                    PropertyAddress = property?.Address,
                },
            });
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            var tenant = await FindTenantAsync();
            if (tenant == null) return NotLinked();
            return Ok(await _paymentService.GetPaymentsByTenantAsync(tenant.Id));
        }

        [HttpGet("maintenance-requests")]
        public async Task<IActionResult> GetMaintenanceRequests()
        {
            var tenant = await FindTenantAsync();
            if (tenant == null) return NotLinked();
            return Ok(await _maintenanceService.GetMaintenanceRequestsByTenantAsync(tenant.Id));
        }

        [HttpPost("maintenance-requests")]
        public async Task<IActionResult> CreateMaintenanceRequest([FromBody] MyMaintenanceRequest request)
        {
            var tenant = await FindTenantAsync();
            if (tenant == null) return NotLinked();

            var created = await _maintenanceService.CreateAsync(new MaintenanceRequestCreateRequest
            {
                TenantId = tenant.Id,
                PropertyId = tenant.PropertyId,
                Title = request.Title,
                Description = request.Description,
                RequestPriority = request.Priority,
            });
            return Ok(created);
        }

        private async Task<TenantResponse?> FindTenantAsync()
        {
            var email = KeycloakClaims.VerifiedEmail(User);
            if (email == null) return null;

            var matches = (await _tenantService.GetAllAsync())
                .Where(t => string.Equals(t.Email, email, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (matches.Count > 1)
                _logger.LogWarning("{Count} tenant records share the email of the signed-in user; using the first", matches.Count);
            return matches.FirstOrDefault();
        }

        private NotFoundObjectResult NotLinked() =>
            NotFound(new { message = "This account isn't linked to a tenant record." });
    }
}
