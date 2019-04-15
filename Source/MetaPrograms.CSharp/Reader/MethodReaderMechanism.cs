using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
using MetaPrograms.Statements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace MetaPrograms.CSharp.Reader
{
    public static class MethodReaderMechanism
    {
        public static MethodSignature ReadSignature(CodeModelBuilder modelBuilder, IMethodSymbol symbol)
        {
            return ReadSignature(modelBuilder.GetCodeModel(), symbol);
        }
        
        public static MethodSignature ReadSignature(ImperativeCodeModel codeModel, IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters.Select((p, index) => new MethodParameter {
                Name = p.Name,
                Position = index + 1,
                Type = codeModel.TryGet<TypeMember>(p.Type),
                Modifier = p.GetParameterModifier(),
            });

            var hasReturnType = (
                symbol.MethodKind != MethodKind.Constructor &&
                symbol.MethodKind != MethodKind.StaticConstructor &&
                !symbol.ReturnsVoid);

            var returnValue = (
                hasReturnType
                    ? new MethodParameter {
                        Name = "$retVal",
                        Position = 0,
                        Type = codeModel.TryGet<TypeMember>(symbol.ReturnType),
                        Modifier = symbol.GetReturnValueModifier(),
                    }
                    : null
            );

            return new MethodSignature {
                IsAsync = symbol.IsAsync, 
                ReturnValue = returnValue, 
                Parameters = parameters.ToList()
            };
        }

        public static void ReadBody(CodeModelBuilder model, IMethodSymbol symbol, IFunctionContext destination)
        {
            var syntax = symbol.DeclaringSyntaxReferences
                .Select(syntaxRef => syntaxRef.GetSyntax())
                .FirstOrDefault(node => node is BaseMethodDeclarationSyntax || node is ArrowExpressionClauseSyntax);

            if (syntax != null)
            {
                var reader = new OperationReader(model, destination, syntax);
                reader.ReadMethodBody();

                // destination.Body = new BlockStatement();
                //
                // if (syntax.ExpressionBody != null)
                // {
                //     destination.Body.Statements.Add(new ReturnStatement {
                //         Expression = ExpressionReader.ReadExpression(model, syntax.ExpressionBody.Expression)
                //     });
                // }
                // else if (syntax.Body != null)
                // {
                //     foreach (var statementSyntax in syntax.Body.Statements)
                //     {
                //         destination.Body.Statements.Add(StatementReader.ReadStatement(model, statementSyntax));
                //     }
                // }
            }
        }

        public static MethodReader CreateAccessorMethodReader(CodeModelBuilder modelBuilder, IMethodSymbol accessorSymbol)
        {
            if (accessorSymbol != null)
            {
                var reader = new MethodReader(modelBuilder, accessorSymbol);
                reader.ReadDeclaration();
                return reader;
            }

            return null;
        }
    }
}