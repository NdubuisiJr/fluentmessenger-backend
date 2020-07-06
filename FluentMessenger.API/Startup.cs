using System;
using AutoMapper;
using FluentMessenger.API.DBContext;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using FluentMessenger.API.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using FluentMessenger.API.Utils;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Secret = FluentMessenger.API.Utils.Secret;

namespace FluentMessenger.API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            //Add the use of controllers and views. Chain NewtonsoftJon and xml serializers
            services.AddControllersWithViews(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson(setupAction => {
                setupAction.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            }).AddXmlDataContractSerializerFormatters()
              .ConfigureApiBehaviorOptions(setupAction => {
                  setupAction.InvalidModelStateResponseFactory = context => {
                      var problemDetails = new ValidationProblemDetails(context.ModelState) {
                          Type = "https://api.fluentMessenger.com/modelvalidationproblem",
                          Title = "One or more model validation errors occurred.",
                          Status = StatusCodes.Status422UnprocessableEntity,
                          Detail = "See the errors property for details.",
                          Instance = context.HttpContext.Request.Path
                      };
                      problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                      return new UnprocessableEntityObjectResult(problemDetails) {
                          ContentTypes = { "application/problem+json" }
                      };
                  };
              });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("Secret");
            services.Configure<Secret>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<Secret>();
            var key = Encoding.ASCII.GetBytes(appSettings.SigningKey);
            services.AddAuthentication(authOptions => {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtBearerOptions => {
                jwtBearerOptions.RequireHttpsMetadata = false;
                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters() {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            //Add automapper services
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //Add repository services
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Group>, GroupRepository>();
            services.AddScoped<IRepository<Contact>, ContactRepository>();
            services.AddScoped<IRepository<Message>, MessageRepository>();
            services.AddScoped<IContactMessageRepository<ContactMessagesReceived>, MessageReceivedRepository>();
            services.AddScoped<IContactMessageRepository<ContactMessagesNotReceived>, MessageNotReceivedRepository>();
            services.AddDbContext<FluentDbContext>(options => {
                var connection=Environment.GetEnvironmentVariable("DATABASE_URL");
                if(connection is null)
                    connection=BuildConnectionString();
                options.UseNpgsql(connection);
                Console.WriteLine($"Using DB={connection}");
            });

            //Add SecurityService
            services.AddScoped<ISecurityService, SecurityService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseExceptionHandler(appBuilder => {
                appBuilder.Run(async context => {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                });
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

        private string BuildConnectionString(){
            var host = Environment.GetEnvironmentVariable("HOST");
            var userId=Environment.GetEnvironmentVariable("USER_ID");
            var userPassword=Environment.GetEnvironmentVariable("USER_PASSWORD");
            var database="fluentDB";
            if(host is null){
                host="ec2-35-172-73-125.compute-1.amazonaws.com";
                database="d73m7sgmrg465o";
                userId="koiqzcpxvqugma";
                userPassword=Configuration.GetConnectionString($"connectionPass");
            }
            var connection=$"User ID={userId};Password={userPassword};Host={host};Port=5432;Database={database};Integrated Security=true;Pooling=true;";
            return connection;
        }
    }
}
