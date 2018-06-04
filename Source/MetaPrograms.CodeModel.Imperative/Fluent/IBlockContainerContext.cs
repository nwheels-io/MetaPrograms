using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public interface IBlockContainerContext
    {
        void AttachBlock(IEnumerable<LocalVariable> locals, IEnumerable<AbstractStatement> statements);
    }
}