using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace MetaPrograms.Statements
{
    public class SwitchCaseBlock
    {
        public void AcceptVisitor(StatementVisitor visitor)
        {
            if (ConstantMatch != null)
            {
                ConstantMatch.AcceptVisitor(visitor);
            }

            if (PatternMatchType != null)
            {
                visitor.VisitReferenceToTypeMember(PatternMatchType);
            }

            if (PatternMatchCondition != null)
            {
                PatternMatchCondition.AcceptVisitor(visitor);
            }

            if (Body != null)
            {
                Body.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression ConstantMatch { get; set; }
        public TypeMember PatternMatchType { get; set; }
        public AbstractExpression PatternMatchCondition { get; set; }
        public BlockStatement Body { get; set; }        
    }
}