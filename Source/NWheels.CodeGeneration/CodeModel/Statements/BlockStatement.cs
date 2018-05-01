using System.Collections.Generic;
using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public class BlockStatement : AbstractStatement
    {
        public BlockStatement(params AbstractStatement[] statements)
        {
            this.Locals = new List<LocalVariable>();
            this.Statements = new List<AbstractStatement>(statements);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBlockStatement(this);

            foreach (var statement in this.Statements)
            {
                statement.AcceptVisitor(visitor);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public List<LocalVariable> Locals { get; }
        public List<AbstractStatement> Statements { get; }
    }
}
