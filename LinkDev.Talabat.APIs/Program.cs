using LinkDev.Talabat.APIs.Controllers;
using LinkDev.Talabat.APIs.Extensions;
using LinkDev.Talabat.APIs.Services;
using LinkDev.Talabat.Core.Application.Abstraction;
using LinkDev.Talabat.Core.Application;
using LinkDev.Talabat.Infrastructure.Persistence; 


namespace LinkDev.Talabat.APIs
{
    public class Program
    {
        
        public static async Task Main(string[] args)
        {
            var webApplicationBuilder = WebApplication.CreateBuilder(args);


            #region Configure Services

            // Add services to the container.

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


            #endregion 

            var app = webApplicationBuilder.Build();

            #region Database Initializer

            await app.IntializeStoreContextAsync();

            #endregion


            #region Configure Kestrel Midllwares

            // Configure the HTTP request pipeline.
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
