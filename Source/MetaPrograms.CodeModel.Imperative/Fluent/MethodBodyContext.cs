using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class MethodBodyContext : BlockContext, IBlockContainerContext
    {
        private readonly MemberRef<MethodMemberBase> _methodRef;

        public MethodBodyContext(CodeGeneratorContext context)
        {
            base.Container = this;
            _methodRef = context.PeekStateOrThrow<IMemberRef>().AsRef<MethodMemberBase>();
        }

        public void AttachBlock(IEnumerable<LocalVariable> locals, IEnumerable<AbstractStatement> statements)
        {
            var block = new BlockStatement(locals.ToImmutableList(), statements.ToImmutableList());
            _methodRef.Get().WithBody(block, shouldReplaceSource: true);
        }
    }
}
