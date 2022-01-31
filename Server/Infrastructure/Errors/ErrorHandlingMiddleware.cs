using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Infrastructure.Errors
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                await HandleException(httpContext, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            string error = null;
            switch(ex)
            {
                case ApiException ae:
                    context.Response.StatusCode = (int)ae.StatusCode;
                    error = JsonSerializer.Serialize(
                        new
                        {
                            error = ae.ErrorMessage
                        });
                    break;
                case Exception:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    error = JsonSerializer.Serialize(
                        new
                        {
                            error = "unexpected error"
                        }); ;
                    break;
            }
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(error);
        }

    }

}
