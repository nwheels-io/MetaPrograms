using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class MethodBodyContext : BlockContextBase
    {
        private readonly MemberRef<MethodMemberBase> _methodRef;

        public MethodBodyContext(CodeGeneratorContext context)
        {
            _methodRef = context.PeekStateOrThrow<IMemberRef>().AsRef<MethodMemberBase>();
        }

        protected override void AttachBlock(IEnumerable<LocalVariable> locals, IEnumerable<AbstractStatement> statements)
        {
            var block = new BlockStatement(locals.ToImmutableList(), statements.ToImmutableList());
            _methodRef.Get().WithBody(block, shouldReplaceSource: true);
        }
    }
}
