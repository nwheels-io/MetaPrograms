using System;
using MetaPrograms.Statements;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public static class StatementSyntaxEmitter
    {
        public static StatementSyntax EmitSyntax(AbstractStatement statement)
        {
            if (statement is ReturnStatement statementReturn)
            {
                return SyntaxFactory.ReturnStatement(ExpressionSyntaxEmitter.EmitSyntax(statementReturn.Expression));
            }
            if (statement is BlockStatement statementBlock)
            {
                return statementBlock.ToSyntax();
            }
            if (statement is ThrowStatement statementThrow)
            {
                return SyntaxFactory.ThrowStatement(ExpressionSyntaxEmitter.EmitSyntax(statementThrow.Exception));
            }
            if (statement is ExpressionStatement statementExpression)
            {
                return SyntaxFactory.ExpressionStatement(ExpressionSyntaxEmitter.EmitSyntax(statementExpression.Expression));
            }
            if (statement is VariableDeclarationStatement statementVariable)
            {
                return EmitLocalDeclarationSyntax(statementVariable);
            }
            if (statement is IfStatement statementIf)
            {
                return EmitIfStatementSyntax(statementIf);
            }
            if (statement is LockStatement statementLock)
            {
                return SyntaxFactory.LockStatement(ExpressionSyntaxEmitter.EmitSyntax(statementLock.SyncRoot), statementLock.Body.ToSyntax());
            }

            //TODO: support other types of statements

            throw new NotSupportedException($"Syntax emitter is not supported for statement of type '{statement.GetType().Name}'.");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static StatementSyntax EmitIfStatementSyntax(IfStatement statement)
        {
            var syntax = SyntaxFactory.IfStatement(ExpressionSyntaxEmitter.EmitSyntax(statement.Condition), statement.ThenBlock.ToSyntax());
            
            if (statement.ElseBlock != null)
            {
                syntax = syntax.WithElse(SyntaxFactory.ElseClause(statement.ElseBlock.ToSyntax()));
            }

            return syntax;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static LocalDeclarationStatementSyntax EmitLocalDeclarationSyntax(VariableDeclarationStatement statement)
        {
            var variable = statement.Variable;

            var declaration = (variable.Type != null
                ? SyntaxFactory.VariableDeclaration(variable.Type.GetTypeNameSyntax())
                : SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var")));

            var declarator = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(variable.Name));

            if (statement.InitialValue != null)
            {
                declarator = declarator.WithInitializer(SyntaxFactory.EqualsValueClause(ExpressionSyntaxEmitter.EmitSyntax(statement.InitialValue)));
            }

            return SyntaxFactory.LocalDeclarationStatement(declaration.WithVariables(SyntaxFactory.SingletonSeparatedList(declarator)));
        }
    }
}
