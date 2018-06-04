using System;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

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