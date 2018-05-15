using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class ConstantExpression : AbstractExpression
    {
        public ConstantExpression(
            MemberRef<TypeMember> type, 
            object value) 
            : base(type)
        {
            Value = value;
        }

        public ConstantExpression(
            ConstantExpression source,
            Mutator<MemberRef<TypeMember>>? type = null,
            Mutator<object>? value = null) 
            : base(source, type)
        {
            Value = value.MutatedOrOriginal(source.Value);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitConstantExpression(this);

            if (Value is TypeMember typeMember)
            {
                visitor.VisitReferenceToTypeMember(typeMember);
            }
            //TODO: handle bindings
            //else if (Value is System.Type systemType)
            //{
            //    visitor.VisitReferenceToTypeMember(systemType);
            //}
            //else if (Value != null)
            //{
            //    visitor.VisitReferenceToTypeMember(Value.GetType());
            //}
        }

        public object Value { get; }
    }
}
