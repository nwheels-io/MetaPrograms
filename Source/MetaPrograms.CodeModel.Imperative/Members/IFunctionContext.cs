using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public interface IFunctionContext
    {
        MethodSignature Signature { get; set; }
        BlockStatement Body { get; set; }
    }
}
