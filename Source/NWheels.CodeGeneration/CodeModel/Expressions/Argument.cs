using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class Argument
    {
        public AbstractExpression Expression { get; set; }
        public MethodParameterModifier Modifier { get; set; }
    }
}
