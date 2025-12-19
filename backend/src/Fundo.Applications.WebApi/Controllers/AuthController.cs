using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fundo.Applications.WebApi.Application.DTOs;
using Fundo.Applications.WebApi.Application.Services;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService auth;
        private readonly ILogger<AuthController> logger;

        public AuthController(AuthService auth, ILogger<AuthController> logger)
        {
            this.auth = auth;
            this.logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] AuthRequestDto request)
        {
            if (request == null)
            {
                logger.LogWarning("Register request body is null.");
                return BadRequest(new { message = "Request body is required." });
            }

            try
            {
                logger.LogInformation("Registering user. Email={Email}", request.Email);

                var result = await auth.RegisterAsync(request);

                logger.LogInformation("User registered successfully. Email={Email}", request.Email);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                // Ej: campos requeridos, formato inválido, etc.
                logger.LogWarning(ex, "Register validation error. Email={Email}", request.Email);

                return Problem(
                    title: "Validation error",
                    detail: ex.Message,
                    statusCode: 400);
            }
            catch (InvalidOperationException ex)
            {
                // Ej: usuario ya existe
                logger.LogWarning(ex, "Register business rule violation. Email={Email}", request.Email);

                return Problem(
                    title: "Business rule violation",
                    detail: ex.Message,
                    statusCode: 400);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during register. Email={Email}", request.Email);

                return Problem(
                    title: "Unexpected error",
                    detail: "An unexpected error occurred while registering the user.",
                    statusCode: 500);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] AuthRequestDto request)
        {
            if (request == null)
            {
                logger.LogWarning("Login request body is null.");
                return BadRequest(new { message = "Request body is required." });
            }

            try
            {
                logger.LogInformation("User login attempt. Email={Email}", request.Email);

                var result = await auth.LoginAsync(request);

                logger.LogInformation("User logged in successfully. Email={Email}", request.Email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Credenciales inválidas
                logger.LogWarning(ex, "Invalid login attempt. Email={Email}", request.Email);

                return Problem(
                    title: "Unauthorized",
                    detail: ex.Message,
                    statusCode: 401);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Login validation error. Email={Email}", request.Email);

                return Problem(
                    title: "Validation error",
                    detail: ex.Message,
                    statusCode: 400);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during login. Email={Email}", request.Email);

                return Problem(
                    title: "Unexpected error",
                    detail: "An unexpected error occurred while logging in.",
                    statusCode: 500);
            }
        }
    }
}
