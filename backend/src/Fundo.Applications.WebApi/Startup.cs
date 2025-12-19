using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.Application.Services;
using Fundo.Applications.WebApi.Infrastructure.Auth;
using Fundo.Applications.WebApi.Infrastructure.Persistence;
using Fundo.Applications.WebApi.Infrastructure.Persistence.Repositories;
using Fundo.Applications.WebApi.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Fundo.Applications.WebApi
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup(IConfiguration configuration) => this.configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fundo Loan Management API",
                    Version = "v1"
                });

                // JWT Bearer support in Swagger
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter: Bearer {your JWT token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [securityScheme] = new List<string>()
                });
            });

            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddAutoMapper(typeof(Fundo.Applications.WebApi.Application.Mapping.AutoMapperProfile).Assembly);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<LoanService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<AuthService>();
            services.AddSingleton<JwtTokenService>();

            // JWT
            var key = configuration["Jwt:Key"];
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

            services.AddAuthorization();

            services.AddCors(opt =>
            {
                opt.AddPolicy("frontend", p =>
                    p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext db, ILogger<Startup> logger)
        {
            WaitForDatabaseAsync(db, logger).GetAwaiter().GetResult();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fundo Loan Management API v1");
                    c.RoutePrefix = "swagger";
                });

                DbSeeder.SeedAsync(db).GetAwaiter().GetResult();

            }

            app.UseRouting();

            app.UseCors("frontend");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static async Task WaitForDatabaseAsync(AppDbContext db, ILogger logger)
        {
            const int maxAttempts = 20;
            var delay = TimeSpan.FromSeconds(5);

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    logger.LogInformation("DB check attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);

                    // Abre conexión
                    var canConnect = await db.Database.CanConnectAsync();
                    if (canConnect)
                    {
                        logger.LogInformation("DB is reachable. Applying migrations...");
                        await db.Database.MigrateAsync();
                        logger.LogInformation("Migrations applied successfully.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "DB not ready yet (attempt {Attempt}/{MaxAttempts})", attempt, maxAttempts);
                }

                await Task.Delay(delay);
            }

            throw new Exception("Database is not reachable after multiple attempts.");
        }
    }
}
