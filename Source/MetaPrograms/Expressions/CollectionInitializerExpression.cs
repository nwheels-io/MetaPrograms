using System.Collections.Generic;

namespace MetaPrograms.Expressions
{
    public class CollectionInitializerExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitCollectionInitializerExpression(this);
            
            Items?.ForEach(item => item?.ItemArguments?.ForEach(arg => arg.AcceptVisitor(visitor)));
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteCollectionInitializerExpression(this);
        }

        public List<ItemInitializer> Items { get; set; } = new List<ItemInitializer>();
        
        public class ItemInitializer
        {
            public List<AbstractExpression> ItemArguments { get; set; } = new List<AbstractExpression>();
        }
    }
}