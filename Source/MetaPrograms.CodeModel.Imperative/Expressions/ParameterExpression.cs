using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class ParameterExpression : AbstractExpression
    {
        public ParameterExpression(
            MethodParameter parameter) 
            : base(parameter.Type)
        {
            Parameter = parameter;
        }

        public ParameterExpression(
            ParameterExpression source,
            Mutator<MethodParameter>? parameter = null) 
            : base(source, parameter.MutatedOrOriginal(source.Parameter).Type)
        {
            Parameter = parameter.MutatedOrOriginal(source.Parameter);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitParameterExpression(this);
        }

        public MethodParameter Parameter { get; }
    }
}
