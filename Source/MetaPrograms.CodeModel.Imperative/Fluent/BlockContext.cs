using System;
using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class BlockContext : IDisposable
    {
        private readonly List<LocalVariable> _locals;
        private readonly List<AbstractStatement> _statements;

        public BlockContext(CodeGeneratorContext context)
            : this()
        {
            Container = context.PeekStateOrThrow<IBlockContainerContext>();
        }

        protected BlockContext()
        {
            _locals = new List<LocalVariable>();
            _statements = new List<AbstractStatement>();
        }

        public void AddLocal(LocalVariable local)
        {
            _locals.Add(local);
        }

        public void AddStatement(AbstractStatement statement)
        {
            _statements.Add(statement);
        }
        
        public void Dispose()
        {
            Container.AttachBlock(_locals, _statements);
        }
        
        public IBlockContainerContext Container { get; protected set; }
    }
}
