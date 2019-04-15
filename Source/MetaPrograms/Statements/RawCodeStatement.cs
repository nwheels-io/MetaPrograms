namespace MetaPrograms.Statements
{
    public class RawCodeStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return this;
        }

        public string Code { get; set; }
    }
}