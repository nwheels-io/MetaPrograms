using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class TryCatchBlock
    {
        public TryCatchBlock(
            TypeMember exceptionType, 
            LocalVariable exceptionVariable, 
            BlockStatement body)
        {
            ExceptionType = exceptionType;
            ExceptionVariable = exceptionVariable;
            Body = body;
        }

        public TryCatchBlock(
            TryCatchBlock source,
            Mutator<TypeMember>? exceptionType = null,
            Mutator<LocalVariable>? exceptionVariable = null,
            Mutator<BlockStatement>? body = null)
        {
            ExceptionType = exceptionType.MutatedOrOriginal(source.ExceptionType);
            ExceptionVariable = exceptionVariable.MutatedOrOriginal(source.ExceptionVariable);
            Body = body.MutatedOrOriginal(source.Body);
        }

        public void AcceptVisitor(StatementVisitor visitor)
        {
            if (ExceptionType != null)
            {
                visitor.VisitReferenceToTypeMember(ExceptionType);
            }

            if (ExceptionVariable != null)
            {
                visitor.VisitReferenceToLocalVariable(ExceptionVariable);
            }

            Body.AcceptVisitor(visitor);
        }

        public TypeMember ExceptionType { get; } 
        public LocalVariable ExceptionVariable { get; }
        public BlockStatement Body { get; }
    }
}