using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.Expressions;

namespace MetaPrograms.Members
{
    public class MethodParameter : IAssignable
    {
        public AbstractExpression AsExpression()
        {
            return new ParameterExpression {
                Parameter = this
            };
        }

        public void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitReferenceToMethodParameter(this);
        }

        IAssignable IAssignable.AcceptRewriter(StatementRewriter rewriter)
        {
            return this;
        }

        public IdentifierName Name { get; set; }
        public TupleExpression Tuple { get; set; }
        public int Position { get; set; }
        public TypeMember Type { get; set; }
        public MethodParameterModifier Modifier { get; set; }
        public List<AttributeDescription> Attributes { get; set; } = new List<AttributeDescription>();

        public static MethodParameter ReturnVoid => null;

        public static implicit operator ParameterExpression(MethodParameter source)
        {
            return new ParameterExpression {
                Parameter = source
            };
        }
    }
}
