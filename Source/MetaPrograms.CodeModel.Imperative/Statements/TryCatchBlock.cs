using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class TryCatchBlock
    {
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

        public TypeMember ExceptionType { get; set; } 
        public LocalVariable ExceptionVariable { get; set; }
        public BlockStatement Body { get; set; }
    }
}