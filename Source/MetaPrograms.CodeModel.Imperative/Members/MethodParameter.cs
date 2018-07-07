using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
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

        public string Name { get; set; }
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
