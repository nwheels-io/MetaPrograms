using System.Collections.Generic;

namespace MetaPrograms.Expressions
{
    public class InterpolatedStringExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitInrerpolatedStringExpression(this);
            Parts?.ForEach(p => {
                switch (p)
                {
                    case TextPart text:
                        //text.Text?.AcceptVisitor(visitor);
                        break;
                    case InterpolationPart interpolation:
                        interpolation.Value?.AcceptVisitor(visitor);
                        interpolation.Alignment?.AcceptVisitor(visitor);
                        interpolation.FormatString?.AcceptVisitor(visitor);
                        break;
                }
            });
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteInterpolatedStringExpression(this);
        }
        
        public List<Part> Parts { get; set; } = new List<Part>();

        public abstract class Part
        {
        }

        public class TextPart : Part
        {
            public string Text { get; set; }
        }

        public class InterpolationPart : Part
        {
            public AbstractExpression Value { get; set; }
            public AbstractExpression Alignment { get; set; }
            public AbstractExpression FormatString { get; set; }
        }
    }
}