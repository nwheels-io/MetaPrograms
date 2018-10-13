using CommonExtensions;
using MetaPrograms;
using MetaPrograms.Members;
using static MetaPrograms.Fluent.Generator;

namespace Example.AspNetAdapter
{
    public static class DataTransferObjectGenerator
    {
        public static TypeMember MethodInvocation(MethodMember method) => 
            PUBLIC.CLASS($"{method.Name}Invocation", () => {
                method.Signature.Parameters.ForEach(p => {
                    PUBLIC.PROPERTY(p.Type, p.Name.ToString(CasingStyle.Pascal));
                });
            });
    }
}