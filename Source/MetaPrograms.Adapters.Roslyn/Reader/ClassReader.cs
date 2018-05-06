using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class ClassReader
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly TypeMemberBuilder _memberBuilder;
        private readonly SemanticModel _semanticModel;
        private readonly ClassDeclarationSyntax _syntax;
        private readonly INamedTypeSymbol _symbol;
        
        public ClassReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, ClassDeclarationSyntax syntax)
        {
            _modelBuilder = modelBuilder;
            _memberBuilder = new TypeMemberBuilder();
            _semanticModel = semanticModel;
            _syntax = syntax;
            _symbol = semanticModel.GetDeclaredSymbol(syntax);
        }

        public void Read()
        {
            _memberBuilder.Namespace = _symbol.GetFullNamespaceName();
            _memberBuilder.Name = _symbol.Name;
            _memberBuilder.IsGenericType = _symbol.IsGenericType;

            if (_symbol.IsGenericType)
            {
                //_memberBuilder.GenericTypeArguments.AddRange(_symbol.TypeArguments.); 
                
            }
            
            var member = new TypeMember(_memberBuilder);
            _modelBuilder.RegisterMember(member, isTopLevel: member.DeclaringType == null);
        }
    }
}
