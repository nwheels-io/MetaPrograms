using CommonExtensions;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.CodeGenerator;

namespace Example.AspNetAdapter
{
    public static class DataTransferObjectGenerator
    {
        public static TypeMember MethodInvocation(MethodMember method) => 
            PUBLIC.CLASS($"{method.Name}Invocation", () => {
                method.Signature.Parameters.ForEach(p => {
                    PUBLIC.PROPERTY(p.Type, p.Name.ToPascalCase(), AUTOMATIC);
                });
            });
    }
}