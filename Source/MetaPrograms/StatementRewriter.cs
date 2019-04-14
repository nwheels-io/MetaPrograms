using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using MetaPrograms.Statements;

namespace MetaPrograms
{
    public class StatementRewriter
    {
        private readonly Dictionary<LocalVariable, LocalVariable> _localVariableReplacements = new Dictionary<LocalVariable, LocalVariable>();
        private readonly Dictionary<MethodParameter, MethodParameter> _methodParameterReplacements = new Dictionary<MethodParameter, MethodParameter>();
        private readonly Dictionary<TypeMember, TypeMember> _typeMemberReplacements = new Dictionary<TypeMember, TypeMember>();

        public virtual AbstractExpression RewriteExpression(AbstractExpression expression)
        {
            return expression.AcceptRewriter(this);
        }

        public virtual BlockStatement RewriteBlockStatement(BlockStatement statement)
        {
            var newVariables = statement.Locals.Select(RewriteLocalVariable).ToList();
            var anyNewVariables = false;

            for (int i = 0; i < newVariables.Count; i++)
            {
                if (newVariables[i] != statement.Locals[i])
                {
                    anyNewVariables = true;
                    AddLocalVariableReplacement(from: statement.Locals[i], to: newVariables[i]);
                }
            }

            var newStatements = statement.Statements.Select(s => s.AcceptRewriter(this)).ToList();
            var anyNewStatements = newStatements.Where((s, index) => s != statement.Statements[index]).Any();

            if (anyNewVariables || anyNewStatements)
            {
                return new BlockStatement {
                    Bindings = new BindingCollection(statement.Bindings),
                    Locals = newVariables,
                    Statements = newStatements
                };
            }

            return statement;
        }

        public virtual DoStatement RewriteDoStatement(DoStatement statement)
        {
            return statement;
        }

        public virtual ExpressionStatement RewriteExpressionStatement(ExpressionStatement statement)
        {
            var newExpression = statement.Expression.AcceptRewriter(this);

            if (newExpression != statement.Expression)
            {
                return new ExpressionStatement {
                    Bindings = new BindingCollection(statement.Bindings),
                    Expression = newExpression
                };
            }

            return statement;
        }

        public virtual ForEachStatement RewriteForEachStatement(ForEachStatement statement)
        {
            return statement;
        }

        public virtual ForStatement RewriteForStatement(ForStatement statement)
        {
            return statement;
        }

        public virtual IfStatement RewriteIfStatement(IfStatement statement)
        {
            return statement;
        }

        public virtual LockStatement RewriteLockStatement(LockStatement statement)
        {
            return statement;
        }

        public virtual PropagateCallStatement RewritePropagateCallStatement(PropagateCallStatement statement)
        {
            return statement;
        }

        public virtual ReThrowStatement RewriteReThrowStatement(ReThrowStatement statement)
        {
            return statement;
        }

        public virtual ReturnStatement RewriteReturnStatement(ReturnStatement statement)
        {
            var newExpression = statement.Expression?.AcceptRewriter(this);

            if (newExpression != statement.Expression)
            {
                return new ReturnStatement {
                    Expression = newExpression
                };
            }

            return statement;
        }

        public virtual SwitchStatement RewriteSwitchStatement(SwitchStatement statement)
        {
            return statement;
        }

        public virtual ThrowStatement RewriteThrowStatement(ThrowStatement statement)
        {
            return statement;
        }

        public virtual TryStatement RewriteTryStatement(TryStatement statement)
        {
            return statement;
        }

        public virtual UsingStatement RewriteUsingStatement(UsingStatement statement)
        {
            return statement;
        }

        public virtual VariableDeclarationStatement RewriteVariableDeclaraitonStatement(VariableDeclarationStatement statement)
        {
            return statement;
        }

        public virtual WhileStatement RewriteWhileStatement(WhileStatement statement)
        {
            return statement;
        }

        public virtual AnonymousDelegateExpression RewriteAnonymousDelegateExpression(AnonymousDelegateExpression expression)
        {
            var newBody = (BlockStatement)expression.Body.AcceptRewriter(this);

            if (newBody != expression.Body)
            {
                return new AnonymousDelegateExpression {
                    Bindings = new BindingCollection(expression.Bindings),
                    Body = newBody,
                    Signature = expression.Signature,
                    Type = expression.Type
                };
            }

            return expression;
        }

        public virtual AssignmentExpression RewriteAssignmentExpression(AssignmentExpression expression)
        {
            var newLeft = expression.Left.AcceptRewriter(this);
            var newRight = expression.Right.AcceptRewriter(this);

            if (newLeft != expression.Left || newRight != expression.Right)
            {
                return new AssignmentExpression {
                    Bindings = new BindingCollection(expression.Bindings),
                    CompoundOperator = expression.CompoundOperator,
                    Type = newRight.Type,
                    Left = newLeft,
                    Right = newRight
                };
            }

            return expression;
        }

        public virtual BaseExpression RewriteBaseExpression(BaseExpression expression)
        {
            return expression;
        }

        public virtual BinaryExpression RewriteBinaryExpression(BinaryExpression expression)
        {
            return expression;
        }

        public virtual CastExpression RewriteCastExpression(CastExpression expression)
        {
            return expression;
        }

        public virtual AwaitExpression RewriteAwaitExpression(AwaitExpression expression)
        {
            var newExpression = expression.Expression.AcceptRewriter(this);

            if (newExpression != expression.Expression)
            {
                return new AwaitExpression {
                    Bindings = new BindingCollection(expression.Bindings),
                    Expression = newExpression,
                    Type = expression.Type
                };
            }

            return expression;
        }

        public virtual TupleExpression RewriteTupleExpression(TupleExpression expression)
        {
            return expression;
        }

        public virtual ConstantExpression RewriteConstantExpression(ConstantExpression expression)
        {
            return expression;
        }

        public virtual InterpolatedStringExpression RewriteInterpolatedStringExpression(InterpolatedStringExpression expression)
        {
            return expression;
        }

        public virtual IndexerExpression RewriteIndexerExpression(IndexerExpression expression)
        {
            return expression;
        }

        public virtual IsExpression RewriteIsExpression(IsExpression expression)
        {
            return expression;
        }

        public virtual LocalVariableExpression RewriteLocalVariableExpression(LocalVariableExpression expression)
        {
            return expression;
        }

        public virtual AbstractExpression RewriteMemberExpression(MemberExpression expression)
        {
            var newTarget = expression.Target?.AcceptRewriter(this);

            if (newTarget != expression.Target)
            {
                return new MemberExpression {
                    Bindings = new BindingCollection(expression.Bindings),
                    Member = expression.Member,
                    MemberName = expression.MemberName,
                    Type = expression.Type,
                    Target = newTarget
                };
            }

            return expression;
        }

        public virtual MethodCallExpression RewriteMethodCallExpression(MethodCallExpression expression)
        {
            var newTarget = expression.Target?.AcceptRewriter(this);
            var newArguments = RewriteArguments(expression.Arguments);

            if (newTarget != expression.Target || newArguments != expression.Arguments)
            {
                return new MethodCallExpression {
                    Bindings = new BindingCollection(expression.Bindings),
                    Target = newTarget,
                    Method = expression.Method,
                    MethodName = expression.MethodName,
                    Arguments = newArguments,
                    Type = expression.Type                    
                };
            }

            return expression;
        }

        public virtual DelegateInvocationExpression RewriteDelegateInvocationExpression(DelegateInvocationExpression expression)
        {
            var newDelegate = expression.Delegate.AcceptRewriter(this);
            var newArguments = RewriteArguments(expression.Arguments);

            if (newDelegate != expression.Delegate || newArguments != expression.Arguments)
            {
                return new DelegateInvocationExpression {
                    Bindings = new BindingCollection(expression.Bindings),
                    Delegate = newDelegate,
                    Arguments = newArguments,
                    Type = expression.Type
                };
            }

            return expression;
        }

        public virtual NewArrayExpression RewriteNewArrayExpression(NewArrayExpression expression)
        {
            return expression;
        }

        public virtual NewObjectExpression RewriteNewObjectExpression(NewObjectExpression expression)
        {
            return expression;
        }

        public virtual ObjectInitializerExpression RewriteObjectInitializerExpression(ObjectInitializerExpression expression)
        {
            return expression;
        }

        public virtual CollectionInitializerExpression RewriteCollectionInitializerExpression(CollectionInitializerExpression expression)
        {
            return expression;
        }

        public virtual ParameterExpression RewriteParameterExpression(ParameterExpression expression)
        {
            return expression;
        }

        public virtual ThisExpression RewriteThisExpression(ThisExpression expression)
        {
            return expression;
        }

        public virtual UnaryExpression RewriteUnaryExpression(UnaryExpression expression)
        {
            return expression;
        }

        public virtual NullExpression RewriteNullExpression(NullExpression expression)
        {
            return expression;
        }

        public virtual XmlExpression RewriteXmlExpression(XmlExpression expression)
        {
            return expression;
        }

        public virtual LocalVariable RewriteLocalVariable(LocalVariable variable)
        {
            return variable;
        }

        public virtual LocalVariable RewriteReferenceToLocalVariable(LocalVariable variable)
        {
            if (_localVariableReplacements.TryGetValue(variable, out var replacement))
            {
                return replacement;
            }

            return variable;
        }

        public virtual MethodParameter RewriteReferenceToMethodParameter(MethodParameter parameter)
        {
            if (_methodParameterReplacements.TryGetValue(parameter, out var replacement))
            {
                return replacement;
            }

            return parameter;
        }

        public virtual TypeMember RewriteReferenceToTypeMember(TypeMember type)
        {
            if (_typeMemberReplacements.TryGetValue(type, out var replacement))
            {
                return replacement;
            }

            return type;
        }

        protected virtual List<Argument> RewriteArguments(List<Argument> arguments)
        {
            List<Argument> newArguments = null;

            for (int i = 0; i < arguments.Count; i++)
            {
                var newExpression = arguments[i].Expression.AcceptRewriter(this);

                if (newExpression != arguments[i].Expression)
                {
                    if (newArguments == null)
                    {
                        newArguments = new List<Argument>(arguments);
                    }

                    newArguments[i] = new Argument {
                        Modifier = arguments[i].Modifier,
                        Expression = newExpression
                    };
                }
            }

            return newArguments ?? arguments;
        }

        protected void AddTypeMemberReplacement(TypeMember from, TypeMember to)
        {
            _typeMemberReplacements.Add(from, to);
        }

        protected void AddMethodParameter(MethodParameter from, MethodParameter to)
        {
            _methodParameterReplacements.Add(from, to);
        }

        protected void AddLocalVariableReplacement(LocalVariable from, LocalVariable to)
        {
            _localVariableReplacements.Add(from, to);
        }
    }
}
