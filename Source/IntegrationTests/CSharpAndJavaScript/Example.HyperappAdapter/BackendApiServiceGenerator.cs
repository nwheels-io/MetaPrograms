using System;
using System.Collections.Generic;
using System.Text;
using CommonExtensions;
using Example.WebUIModel.Metadata;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

namespace Example.HyperappAdapter
{
    public class BackendApiServiceGenerator
    {
        public static ModuleMember BackendApiService(WebApiMetadata api)
        {
            var serviceName = $"{api.InterfaceType.Name.TrimPrefix("I")}";

            return MODULE(new[] { "src", "services" }, serviceName, () => {
                IMPORT.DEFAULT("axios", out var @axios).FROM("axios");

                EXPORT.DEFAULT.CLASS(serviceName, () => {
                    api.ApiMethods.ForEach(apiMethod => {

                        var methodUrl = api.GetMethodUrl(apiMethod);
                        
                        //TODO: separate language-specific parts of fluent code DSL
                        //For instance, 'PUBLIC' and 'PRIVATE' have no meaning in JavaScript
                        
                        PUBLIC.STATIC.FUNCTION(apiMethod.Name, () => {
                            PARAMETER("parameters", out var @parameters);

                            DO.RETURN(@axios
                                .DOT("post").INVOKE(ANY(methodUrl), @parameters)
                                .DOT("then").INVOKE(LAMBDA(@result =>
                                    DO.RETURN(@result.DOT("data"))
                                ))
                                .DOT("catch").INVOKE(LAMBDA(@err => {
                                    USE("console").DOT("log").INVOKE(@err);
                                }))
                            );
                        });
                    });
                });
            });
        }
    }
}
