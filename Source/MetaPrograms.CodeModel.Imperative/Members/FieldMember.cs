using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class FieldMember : AbstractMember
    {
        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitField(this);
        }

        public MemberExpression AsThisMemberExpression()
        {
            var target = (
                Modifier != MemberModifier.Static 
                ? new ThisExpression { Type = DeclaringType } 
                : null);
            
            return new MemberExpression {
                Type = this.Type, 
                Target = target, 
                Member =  this
            };
        }

        public TypeMember Type { get; set; }
        public bool IsReadOnly { get; set; }
        public AbstractExpression Initializer { get; set; }
    }
}
