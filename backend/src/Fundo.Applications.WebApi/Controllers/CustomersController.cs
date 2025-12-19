using System;
using System.Threading;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("customers")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService service;
        private readonly ILogger<CustomersController> logger;

        public CustomersController(CustomerService service, ILogger<CustomersController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerRequestDto dto, CancellationToken ct)
        {
            if (dto == null)
            {
                logger.LogWarning("Create customer request body is null.");
                return BadRequest(new { message = "Request body is required." });
            }

            try
            {
                logger.LogInformation("Creating customer. Payload={@Payload}", dto);

                var created = await service.CreateAsync(dto, ct);

                logger.LogInformation("Customer created successfully. CustomerId={CustomerId} Email={Email} FullName={FullName}",
                    created.Id, created.Email, created.FullName);

                return CreatedAtAction(nameof(GetAll), new { }, created);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Create customer validation error. Payload={@Payload}", dto);

                return Problem(
                    title: "Validation error",
                    detail: ex.Message,
                    statusCode: 400);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Create customer business rule violation. Payload={@Payload}", dto);

                return Problem(
                    title: "Business rule violation",
                    detail: ex.Message,
                    statusCode: 400);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Create customer request cancelled.");
                return Problem(
                    title: "Request cancelled",
                    detail: "The request was cancelled.",
                    statusCode: 499);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error creating customer. Payload={@Payload}", dto);

                return Problem(
                    title: "Unexpected error",
                    detail: "An unexpected error occurred while creating the customer.",
                    statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(CancellationToken ct)
        {
            try
            {
                logger.LogInformation("Listing customers.");

                var customers = await service.GetAllAsync(ct);

                logger.LogInformation("Customers listed successfully.");

                return Ok(customers);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("GetAll customers request cancelled.");
                return Problem(
                    title: "Request cancelled",
                    detail: "The request was cancelled.",
                    statusCode: 499);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error listing customers.");

                return Problem(
                    title: "Unexpected error",
                    detail: "An unexpected error occurred while listing customers.",
                    statusCode: 500);
            }
        }
    }
}
