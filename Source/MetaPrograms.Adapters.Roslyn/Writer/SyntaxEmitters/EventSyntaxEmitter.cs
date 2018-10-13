using System;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public class EventSyntaxEmitter : MemberSyntaxEmitterBase<EventMember, EventDeclarationSyntax>
    {
        public EventSyntaxEmitter(EventMember @event) :
            base(@event)
        {
        }

        public override EventDeclarationSyntax EmitSyntax()
        {
            throw new NotImplementedException();
        }
    }
}