using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class NewObjectExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitNewObjectExpression(this);

            if (ConstructorCall != null)
            {
                ConstructorCall.AcceptVisitor(visitor);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteNewObjectExpression(this);
        }

        public MethodCallExpression ConstructorCall { get; set; }
        public ObjectInitializerExpression ObjectInitializer { get; set; }
        public CollectionInitializerExpression CollectionInitializer { get; set; }
    }
}
