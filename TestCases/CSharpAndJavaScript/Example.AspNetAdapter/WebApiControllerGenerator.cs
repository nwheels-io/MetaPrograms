using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using CommonExtensions;
using Example.WebUIModel.Metadata;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.AspNetCore.Mvc;
using static MetaPrograms.Fluent.Generator;

// ReSharper disable InconsistentNaming

namespace Example.AspNetAdapter
{
    public static class WebApiControllerGenerator
    {
        //TODO: add detection & resolution of duplicate names
        public static TypeMember WebApiController(TypeMember middlewareType, WebApiMetadata api) =>
            PUBLIC.CLASS(ID(api.InterfaceType.Name.TrimPrefixFragment("I"), "Controller"), () => {
                EXTENDS<Controller>();
                ATTRIBUTE<RouteAttribute>("api/[controller]");

                PRIVATE.READONLY.FIELD(api.InterfaceType, "_service", out var @serviceField);

                PUBLIC.CONSTRUCTOR(() => {
                    PARAMETER(api.InterfaceType, "service", out var @service);
                    @serviceField.ASSIGN(@service);
                });

                api.ApiMethods.ForEach(apiMethod => {
                    var requestClass = DataTransferObjectGenerator.MethodInvocation(apiMethod);

                    PUBLIC.ASYNC.FUNCTION<Task<IActionResult>>(apiMethod.Name, () => {
                        ATTRIBUTE(middlewareType);
                        ATTRIBUTE<HttpPostAttribute>(apiMethod.Name.ToString(CasingStyle.Camel));
                        PARAMETER(requestClass, "requestData", out MethodParameter @requestData, () => {
                            ATTRIBUTE<FromBodyAttribute>();
                        });

                        LOCAL(apiMethod.ReturnType.GenericArguments[0], "resultValue", out LocalVariable resultValueLocal);

                        resultValueLocal.ASSIGN(
                            AWAIT(THIS.DOT(@serviceField).DOT(apiMethod).INVOKE(() => {
                                apiMethod.Signature.Parameters.ForEach(p => ARGUMENT(@requestData.DOT(p.Name.ToString(CasingStyle.Pascal))));
                            }))
                        );

                        DO.RETURN(THIS.DOT("Json").INVOKE(resultValueLocal));
                    });
                });
            });
    }
}