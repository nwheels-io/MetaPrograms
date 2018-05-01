using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.Parsing
{
    public class CSharpCodeParser
    {
        private readonly List<TypeMember> _types = new List<TypeMember>();

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void Parse(Stream input)
        {
            var source = SourceText.From(input);
            var syntax = CSharpSyntaxTree.ParseText(source);
            var topLevelClasses = syntax.GetCompilationUnitRoot()
                .DescendantNodes(descendIntoChildren: node => node is NamespaceDeclarationSyntax)
                .OfType<ClassDeclarationSyntax>();

            foreach (var singleClass in topLevelClasses)
            {
                _types.Add(ParseClass(singleClass));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public IEnumerable<TypeMember> Types => _types;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private TypeMember ParseClass(ClassDeclarationSyntax syntax)
        {
            return new TypeMember();
        }
    }
}
