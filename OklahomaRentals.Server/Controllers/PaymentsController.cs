using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        // GET /api/payments/property/{propertyId}/incoming
        [HttpGet("property/{propertyId}/incoming")]
        public async Task<IActionResult> GetIncomingPayments(int propertyId)
        {
            var payments = await _paymentService.GetIncomingPaymentsAsync(propertyId);
            return Ok(payments);
        }

        // GET /api/payments/property/{propertyId}/outstanding-balances
        [HttpGet("property/{propertyId}/outstanding-balances")]
        public async Task<IActionResult> GetTenantsWithOutstandingBalances(int propertyId)
        {
            var tenants = await _paymentService.GetTenantsWithOutstandingBalancesAsync(propertyId);
            return Ok(tenants);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentResponse payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPayment = await _paymentService.CreateAsync(payment);

            return CreatedAtAction(nameof(GetById), new { id = createdPayment.Id }, createdPayment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentResponse payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _paymentService.UpdateAsync(id, payment);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _paymentService.DeleteAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}