using System;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static MetaPrograms.Fluent.Generator;

namespace Example.AspNetAdapter
{
    public static class AspNetMiddlewareGenerator
    {
        public static TypeMember InvalidModelAutoResponderAttribute() =>
            PUBLIC.CLASS("InvalidModelAutoResponderAttribute", () => {
                EXTENDS<ActionFilterAttribute>();
                
                PUBLIC.OVERRIDE.VOID("OnActionExecuting", () => {
                    PARAMETER<ActionExecutingContext>("context", out MethodParameter @context);
                    
                    DO.IF(NOT(context.DOT("ModelState").DOT("Valid"))).THEN(() => {
                        @context.DOT("Result").ASSIGN(NEW<BadRequestObjectResult>(@context.DOT("ModelState")));
                    });
                });
            });
    }
}