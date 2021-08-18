using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DWorldProject.Utils.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILogger Log = Serilog.Log.ForContext<ExceptionMiddleware>();

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleGlobalExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleGlobalExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var now = DateTime.Now;
            Log.Error($"{now.ToString("HH:mm:ss")} : {exception.Message}");
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new GlobalErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            }));
        }

    }

    public class GlobalErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
