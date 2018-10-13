namespace MetaPrograms.Expressions
{
    public enum UnaryOperator
    {
        LogicalNot,
        BitwiseNot,
        Plus,
        Negation,
        PreIncrement,
        PostIncrement,
        PreDecrement,
        PostDecrement
    }

    public static class UnaryOperatorExtensions
    {
        public static bool IsPostfix(this UnaryOperator op)
        {
            return (op == UnaryOperator.PostIncrement || op == UnaryOperator.PostDecrement);
        }

        public static bool IsPrefix(this UnaryOperator op)
        {
            return !IsPostfix(op);
        }
    }
}
