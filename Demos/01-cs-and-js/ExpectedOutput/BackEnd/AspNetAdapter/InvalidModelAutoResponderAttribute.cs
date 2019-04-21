using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Example.AspNetAdapter
{
    public class InvalidModelAutoResponderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.Valid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}