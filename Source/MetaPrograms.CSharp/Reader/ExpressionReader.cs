using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Reader
{
    public static class ExpressionReader
    {
        public static ConstantExpression ReadTypedConstant(CodeModelBuilder model, TypedConstant constant)
        {
            return new ConstantExpression {
                Type = model.TryGetMember<TypeMember>(constant.Type),
                Value = constant.Value
            };
        }
    }
}

#if false

//TO BE REPLACED BY OperationReader

using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Reader
{
    public static class ExpressionReader
    {
        private static readonly Dictionary<Type, Func<CodeModelBuilder, ExpressionSyntax, AbstractExpression>> ReaderBySyntaxType =
            new Dictionary<Type, Func<CodeModelBuilder, ExpressionSyntax, AbstractExpression>> {
                [typeof(AssignmentExpressionSyntax)] = (m, s) => ReadAssignment(m, (AssignmentExpressionSyntax)s),
                [typeof(MemberAccessExpressionSyntax)] = (m, s) => ReadMemberAccess(m, (MemberAccessExpressionSyntax)s),
                [typeof(ParenthesizedLambdaExpressionSyntax)] = (m, s) => ReadParenthesizedLambda(m, (ParenthesizedLambdaExpressionSyntax)s),
            };

        public static ConstantExpression ReadTypedConstant(CodeModelBuilder model, TypedConstant constant)
        {
            return new ConstantExpression {
                Type = model.TryGetMember<TypeMember>(constant.Type),
                Value = constant.Value
            };
        }

        public static AbstractExpression ReadExpression(CodeModelBuilder model, ExpressionSyntax syntax)
        {
            if (ReaderBySyntaxType.TryGetValue(syntax.GetType(), out var reader))
            {
                return reader(model, syntax);
            }
            else
            {
                throw new NotSupportedException(
                    $"Expression of type '{syntax.GetType().Name}' is not supported by {nameof(ExpressionReader)}.");
            }
        }

        private static AbstractExpression ReadAssignment(CodeModelBuilder model, AssignmentExpressionSyntax syntax)
        {
            return new AssignmentExpression {
                Left = (IAssignable)ReadExpression(model, syntax.Left),
                Right = ReadExpression(model, syntax.Right),
                //TODO: initialize Type
            };
        }

        private static AbstractExpression ReadMemberAccess(CodeModelBuilder model, MemberAccessExpressionSyntax syntax)
        {
            //TODO: implement this
            return new MemberExpression {
                
            };
        }

        private static AbstractExpression ReadParenthesizedLambda(CodeModelBuilder model, ParenthesizedLambdaExpressionSyntax syntax)
        {
            var semanticModel = model.GetSemanticModel(syntax);
            var returnType = model.TryGetMember<TypeMember>(semanticModel.GetTypeInfo(syntax).Type); 
            
            
            
            //TODO: implement this
            var invocation = new DelegateInvocationExpression() {
                Delegate = new AnonymousDelegateExpression {
                    Signature = ReadSignature(syntax.ParameterList, returnType),
                    Type = returnType 
                } 
            };

            
            MethodReaderMechanism.ReadSignature()
            Body = MethodReaderMechanism.ReadBody()
                
            return invocation;
        }
    }
}

#endif