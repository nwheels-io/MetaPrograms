using MetaPrograms.Statements;

namespace MetaPrograms.Members
{
    public interface IFunctionContext
    {
        MethodSignature Signature { get; set; }
        BlockStatement Body { get; set; }
    }
}
