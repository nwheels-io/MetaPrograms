using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class SwitchCaseBlock
    {
        public SwitchCaseBlock(
            AbstractExpression constantMatch, 
            TypeMember patternMatchType, 
            AbstractExpression patternMatchCondition, 
            BlockStatement body)
        {
            ConstantMatch = constantMatch;
            PatternMatchType = patternMatchType;
            PatternMatchCondition = patternMatchCondition;
            Body = body;
        }

        public SwitchCaseBlock(
            SwitchCaseBlock source,
            Mutator<AbstractExpression>? constantMatch = null,
            Mutator<TypeMember>? patternMatchType = null,
            Mutator<AbstractExpression>? patternMatchCondition = null,
            Mutator<BlockStatement>? body = null)
        {
            ConstantMatch = constantMatch.MutatedOrOriginal(source.ConstantMatch);
            PatternMatchType = patternMatchType.MutatedOrOriginal(source.PatternMatchType);
            PatternMatchCondition = patternMatchCondition.MutatedOrOriginal(source.PatternMatchCondition);
            Body = body.MutatedOrOriginal(source.Body);
        }

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

        public AbstractExpression ConstantMatch { get; }
        public TypeMember PatternMatchType { get; }
        public AbstractExpression PatternMatchCondition { get; }
        public BlockStatement Body { get; }        
    }
}