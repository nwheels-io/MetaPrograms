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
        public static TypeMember BackendApiService(WebApiMetadata api)
        {
            var serviceName = $"{api.InterfaceType.Name.TrimPrefix("I")}";

            return MODULE(serviceName, () => {
                IMPORT.DEFAULT("axios", out var @axios).FROM("axios");

                EXPORT.DEFAULT.CLASS(serviceName, () => {
                    api.ApiMethods.ForEach(apiMethod => {
                        PUBLIC.STATIC.FUNCTION(apiMethod.Name, () => {
                            PARAMETER("parameters", out var @parameters);

                            DO.RETURN(@axios
                                .DOT("post").INVOKE(ANY(apiMethod.Url), @parameters)
                                //.DOT("then").INVOKE(.....)
                                //.DOT("catch").INVOKE(.....)
                            );
                        });
                    });
                });
            });
        }
    }
}
