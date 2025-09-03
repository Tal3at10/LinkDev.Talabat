using LinkDev.Talabat.APIs.Extensions;
using LinkDev.Talabat.APIs.Services;
using LinkDev.Talabat.Core.Application.Abstraction;
using LinkDev.Talabat.Core.Application;
using LinkDev.Talabat.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using LinkDev.Talabat.Infrastructure;

using LinkDev.Talabat.APIs.Controllers.Errors;
using LinkDev.Talabat.APIs.Middlewares;
using LinkDev.Talabat.Infrastruture;


namespace LinkDev.Talabat.APIs
{
    public class Program
    {
        
        public static async Task Main(string[] args)
        {
            var webApplicationBuilder = WebApplication.CreateBuilder(args);


            #region Configure Services

            // Add services to the container.

            webApplicationBuilder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.InvalidModelStateResponseFactory = (ActionContext) =>
                {
                    var errors = ActionContext.ModelState
                  .Where(e => e.Value.Errors.Count > 0)
                  .Select
                (
                   p => new ApiValidationErrorResponse.ValidationError
                   {
                       Field = p.Key,
                       Errors = p.Value!.Errors.Select(e => e.ErrorMessage)
                   }
                );

                   return new BadRequestObjectResult(new ApiValidationErrorResponse()
                   {
                       Errors = errors
                   });

                };
            })
                 .AddApplicationPart(typeof(Controllers.AssemblyInformation).Assembly);

            webApplicationBuilder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            webApplicationBuilder.Services.AddEndpointsApiExplorer();
            webApplicationBuilder.Services.AddSwaggerGen();

            webApplicationBuilder.Services.AddHttpContextAccessor();

            webApplicationBuilder.Services.AddScoped(typeof(ILoggedInUserService), typeof(LoggedInUserService));

            webApplicationBuilder.Services.AddPersistenceServices(webApplicationBuilder.Configuration);

            webApplicationBuilder.Services
                .AddControllers()
                .AddApplicationPart(typeof(Controllers.AssemblyInformation).Assembly);

            webApplicationBuilder.Services.AddApplicationServices();
            webApplicationBuilder.Services.AddInfrastructureServices(webApplicationBuilder.Configuration);

            #endregion 

            var app = webApplicationBuilder.Build();

            #region Database Initializer

            await app.IntializeStoreContextAsync();

            #endregion


            #region Configure Kestrel Midllwares

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
