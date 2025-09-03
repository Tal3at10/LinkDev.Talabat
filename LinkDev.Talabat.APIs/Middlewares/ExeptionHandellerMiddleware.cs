using System.Net;
using System.Text.Json;
using LinkDev.Talabat.APIs.Controllers.Errors;
using LinkDev.Talabat.Core.Application.Common.Exeptions;
using LinkDev.Talabat.Core.Application.Exeptions;
using CoreExceptions = LinkDev.Talabat.Core.Application.Common.Exeptions;

namespace LinkDev.Talabat.APIs.Middlewares
{
    // Custom Exception Handler Middleware
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                #region Logging
                if (_env.IsDevelopment())
                {
                    _logger.LogError(ex, ex.Message);
                    //var  response = new ApiExeptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace?.ToString());
                }

                else
                {
                    //response = new ApiExeptionResponse((int)HttpStatusCode.InternalServerError);
                }
                await HandleExceptionAsync(httpContext, ex); 
                #endregion
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            ApiResponse response; 
            switch (ex)
            {
                case NotFoundExeption:
                    
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    httpContext.Response.ContentType = "application/json";
                    response = new ApiResponse(404, ex.Message);
                    await httpContext.Response.WriteAsync(response.ToString());
                    break;

                    case BadRequestExeption:

                        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        httpContext.Response.ContentType = "application/json";
                        response = new ApiResponse(400, ex.Message);
                        await httpContext.Response.WriteAsync(response.ToString());
                        break;

                default:
                    if (_env.IsDevelopment())
                    {
                       _logger.LogError(ex, ex.Message);
                        response = new ApiExeptionResponse((int)HttpStatusCode.InternalServerError, ex.Message);
                    }
                    else
                    {
                        // Minimal response in Production
                        response = new ApiExeptionResponse((int)HttpStatusCode.InternalServerError);
                    }

                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(response.ToString());

                    break;
            }

       }
    }
}
