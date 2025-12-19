using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Application.Services;
using Microsoft.Extensions.Logging;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("loans")]
    [Authorize]
    public class LoanManagementController : ControllerBase
    {
        private readonly LoanService service;
        private readonly ILogger<LoanManagementController> logger;

        public LoanManagementController(LoanService service, ILogger<LoanManagementController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateLoanRequestDto request)
        {
            if (request == null)
            {
                logger.LogWarning("Create loan request body is null.");
                return BadRequest(new { message = "Request body is required." });
            }

            try
            {
                logger.LogInformation("Creating loan. Payload={@Payload}", request);

                var created = await service.CreateAsync(request);

                logger.LogInformation(
                    "Loan created successfully. LoanId={LoanId} Status={Status} OriginalAmount={OriginalAmount} CurrentBalance={CurrentBalance}",
                    created.Id, created.Status, created.OriginalAmount, created.CurrentBalance
                );

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                // Para validaciones/errores de input que tu servicio podría lanzar
                logger.LogWarning(ex, "Create loan validation error. Payload={@Payload}", request);

                return Problem(
                    title: "Validation error",
                    detail: ex.Message,
                    statusCode: 400);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error creating loan. Payload={@Payload}", request);

                return Problem(
                    title: "Unexpected error",
                    detail: "An unexpected error occurred while creating the loan.",
                    statusCode: 500);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            try
            {
                logger.LogInformation("Getting loan by id. LoanId={LoanId}", id);

                var loan = await service.GetByIdAsync(id);

                if (loan == null)
                {
                    logger.LogWarning("Loan not found. LoanId={LoanId}", id);
                    return NotFound(new { message = "Loan not found." });
                }

                logger.LogInformation("Loan retrieved successfully. LoanId={LoanId} Status={Status}", id, loan.Status);
                return Ok(loan);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error getting loan by id. LoanId={LoanId}", id);

                return Problem(
                    title: "Unexpected error",
                    detail: "An unexpected error occurred while retrieving the loan.",
                    statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                logger.LogInformation("Listing loans.");

                var loans = await service.GetAllAsync();

                logger.LogInformation("Loans listed successfully.");

                return Ok(loans);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error listing loans.");

                return Problem(
                    title: "Unexpected error",
                    detail: "An unexpected error occurred while listing loans.",
                    statusCode: 500);
            }
        }

        [HttpPost("{id:guid}/payment")]
        public async Task<ActionResult> Pay(Guid id, [FromBody] LoanPaymentRequestDto request)
        {
            if (request == null)
            {
                logger.LogWarning("Pay request body is null. LoanId={LoanId}", id);
                return BadRequest(new { message = "Request body is required." });
            }

            try
            {
                logger.LogInformation("Applying payment. LoanId={LoanId} Payload={@Payload}", id, request);

                var result = await service.PayAsync(id, request);

                if (result == null)
                {
                    logger.LogWarning("Payment failed - loan not found. LoanId={LoanId}", id);
                    return NotFound(new { message = "Loan not found." });
                }

                logger.LogInformation(
                    "Payment applied successfully. LoanId={LoanId} NewBalance={NewBalance} Status={Status}",
                    id, result.NewBalance, result.Status
                );

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Payment validation error. LoanId={LoanId} Payload={@Payload}", id, request);

                return Problem(
                    title: "Validation error",
                    detail: ex.Message,
                    statusCode: 400);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error applying payment. LoanId={LoanId} Payload={@Payload}", id, request);

                return Problem(
                    title: "Unexpected error",
                    detail: "An unexpected error occurred while applying the payment.",
                    statusCode: 500);
            }
        }
    }
}
