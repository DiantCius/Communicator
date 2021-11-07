using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Infrastructure.Filters
{
    public class ValidatorActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var result = new ContentResult();
                string error = null;

                foreach (var valuePair in context.ModelState)
                {
                    error+= valuePair.Value.Errors.Select(x => x.ErrorMessage).Aggregate((a, b) => a + b) + " ";
                }

                string content = JsonSerializer.Serialize(new { error }); // za kazdym razem nowy error
                result.Content = content;
                result.ContentType = "application/json";

                context.HttpContext.Response.StatusCode = 422; 
                context.Result = result;
                return;
            }

            await next();
        }
    }
}
