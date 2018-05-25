using CommonExtensions;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.AspNetAdapter
{
    public static class MethodCallDtoGenerator
    {
        public static object Generate(CodeModelBuilder codeModel, MethodMember method) => 
            NAMESPACE($"{method.Name}Request", () => {
                USE(model);

                PUBLIC.CLASS($"{type.Name.TrimPrefix("I")}Controller", () => {
                    method.Signature.Parameters.ForEach(p => {
                        PUBLIC.PROPERTY(p.Type, p.Name.ToPascalCase(), () => {
                                                            
                        });
                    });
                });
            });
    }
}