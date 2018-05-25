using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using CommonExtensions;
using Example.WebUIModel.Metadata;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Language.TypeParams;
using static MetaPrograms.CodeModel.Imperative.CodeGenerator;

namespace Example.AspNetAdapter
{
    public static class ApiControllerGenerator
    {
        //TODO: add detection & resolution of duplicate names
        public static object Generate(ImmutableCodeModel model, WebUIMetadata ui, WebApiMetadata api) =>
            NAMESPACE($"{api.InterfaceType.Namespace}.WebApi", () => {
                USE(model);

                PUBLIC.CLASS($"{api.InterfaceType.Name.TrimPrefix("I")}Controller", () => {
                    EXTENDS<Controller>();
                    ATTRIBUTE<RouteAttribute>("api/[controller]");

                    PRIVATE.READONLY.FIELD(api.InterfaceType, "_service", out object serviceField);

                    PUBLIC.CONSTRUCTOR(() => {
                        PARAMETER(api.InterfaceType, "service", out object serviceParameter);
                        DO.ASSIGN(serviceField).FROM(serviceParameter);
                    });

                    api.ApiMethods.ForEach(apiMethod => {
                        var requestClass = MethodCallDtoGenerator.Generate(model, apiMethod);

                        PUBLIC.ASYNC.FUNCTION<IActionResult>(apiMethod.Name, () => {
                            ATTRIBUTE<InvalidModelAutoResponderAttribute>();
                            ATTRIBUTE<HttpPostAttribute>(apiMethod.Name.ToCamelCase());
                            PARAMETER(requestClass, "requestData", out object requestDataParam, () => {
                                ATTRIBUTE<FromBodyAttribute>();
                            });

                            LOCAL(apiMethod.ReturnType.GenericArguments[0], "resultValue", out object resultValueLocal);
                            DO.ASSIGN(resultValueLocal).FROM(AWAIT(serviceField.DOT(apiMethod, () => {
                                apiMethod.Signature.Parameters.ForEach(p => ARGUMENT(requestDataParam.DOT(p.Name.ToPascalCase())));
                            })));

                            DO.RETURN(THIS.DOT("Json").INVOKE(resultValueLocal));
                        });
                    });
                });
            });
    }
}
