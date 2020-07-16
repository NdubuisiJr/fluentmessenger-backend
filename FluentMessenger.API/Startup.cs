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
using Secret = FluentMessenger.API.Utils.Secret;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FluentMessenger.API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            //Add the use of controllers and views. Chain NewtonsoftJon and xml serializers
            services.AddControllersWithViews(setupAction => {
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(
                            StatusCodes.Status500InternalServerError));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(
                            StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(
                            StatusCodes.Status406NotAcceptable));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(
                            StatusCodes.Status401Unauthorized));
                setupAction.Filters.Add(new ProducesAttribute(
                            "application/json", new string[] { "application/xml" }));
                setupAction.Filters.Add(new ConsumesAttribute(
                            "application/json", new string[] { }));
                // Add other global Response StatusCodes

                setupAction.ReturnHttpNotAcceptable = true;

                setupAction.OutputFormatters.Add(new XmlSerializerOutputFormatter());

            }).AddNewtonsoftJson(setupAction => {
                setupAction.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            }).ConfigureApiBehaviorOptions(setupAction => {
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
            _connectionString=BuildConnectionString();
            services.AddDbContext<FluentDbContext>(options => {
                options.UseNpgsql(_connectionString);
                Console.WriteLine($"Using DB={_connectionString}");
            });

            //Add SecurityService
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddSwaggerGen(setupAction => {
                setupAction.SwaggerDoc("Docs",
                    new Microsoft.OpenApi.Models.OpenApiInfo() {
                        Title = "Fluent Docs",
                        Version = "1",
                        Description="This is the backend for the fluent messenger mobile application",
                        Contact= new Microsoft.OpenApi.Models.OpenApiContact {
                            Email="ndubuisijrchukuigwe@gmail.com",
                            Name="Ndubuisi Jr Chukuigwe",
                            Url= new Uri("https://www.github.com/ndubuisijr")
                        }
                    });
                var xmlDocFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var path = Path.Combine(AppContext.BaseDirectory, xmlDocFile);
                setupAction.IncludeXmlComments(path);
            });
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

            app.UseSwagger();
            app.UseSwaggerUI(setupAction => {
                setupAction.SwaggerEndpoint("/swagger/Docs/swagger.json", "Fluent Docs");
                setupAction.RoutePrefix = "";
            });
            app.UseStaticFiles();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

        private string BuildConnectionString(){
            var host = Environment.GetEnvironmentVariable("HOST")??"localhost";
            var userId = Environment.GetEnvironmentVariable("USER_ID")??"postgres";
            var userPassword = Environment.GetEnvironmentVariable("USER_PASSWORD")??"test";
            var database = "fluentDB";
            var connection=$"User ID={userId};Password={userPassword};Server={host};Port=5432;Database={database};Integrated Security=true;Pooling=true;";
            return connection;
        }
        private string _connectionString;
    }
}
