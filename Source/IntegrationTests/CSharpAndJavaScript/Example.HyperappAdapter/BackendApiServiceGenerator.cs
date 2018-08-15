using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonExtensions;
using Example.WebUIModel.Metadata;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

namespace Example.HyperappAdapter
{
    public static class BackendApiServiceGenerator
    {
        public static ModuleMember BackendApiService(WebApiMetadata api)
            => MODULE(new[] { "src", "services" }, ID(api.ServiceName, "Service"), () => {
                IMPORT.DEFAULT("axios", out var @axios).FROM("axios");

                EXPORT.CLASS(ID(api.ServiceName, "Service"), () => {
                    api.ApiMethods.ForEach(apiMethod => {

                        var methodUrl = api.GetMethodUrl(apiMethod);
                        
                        //TODO: separate language-specific parts of fluent code DSL
                        //For instance, 'PUBLIC' and 'PRIVATE' are not defined in JavaScript, and 'IMPORT' is not defined in C#
                        
                        PUBLIC.STATIC.FUNCTION(apiMethod.Name, () => {
                            var arguments = apiMethod.Signature.Parameters.Select((parameter, index) => {
                                PARAMETER(parameter.Name, out var @param);
                                return @param;
                            }).ToList();

                            LOCAL("data", out var @data, INITOBJECT(() => {
                                arguments.ForEach(arg => KEY(arg.Name, arg));
                            }));

                            DO.RETURN(@axios
                                .DOT("post").INVOKE(ANY(methodUrl), @data)
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
