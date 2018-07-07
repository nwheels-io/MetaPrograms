﻿using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative
{
    public abstract class StatementVisitor
    {
        public virtual void VisitBlockStatement(BlockStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitDoStatement(DoStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitExpressionStatement(ExpressionStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitForEachStatement(ForEachStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitForStatement(ForStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitIfStatement(IfStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitLockStatement(LockStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitPropagateCallStatement(PropagateCallStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitReThrowStatement(ReThrowStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitReturnStatement(ReturnStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitSwitchStatement(SwitchStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitThrowStatement(ThrowStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitTryStatement(TryStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitUsingStatement(UsingStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitVariableDeclaraitonStatement(VariableDeclarationStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitWhileStatement(WhileStatement statement)
        {
            VisitAbstractStatement(statement);
        }

        public virtual void VisitAnonymousDelegateExpression(AnonymousDelegateExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitAssignmentExpression(AssignmentExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitBaseExpression(BaseExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitBinaryExpression(BinaryExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitCastExpression(CastExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitAwaitExpression(AwaitExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitTupleExpression(TupleExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitConstantExpression(ConstantExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitIndexerExpression(IndexerExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitIsExpression(IsExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitLocalVariableExpression(LocalVariableExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitMemberExpression(MemberExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitMethodCallExpression(MethodCallExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitNewArrayExpression(NewArrayExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitNewObjectExpression(NewObjectExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitObjectInitializerExpression(ObjectInitializerExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitParameterExpression(ParameterExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitThisExpression(ThisExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitUnaryExpression(UnaryExpression expression)
        {
            VisitAbstractExpression(expression);
        }

        public virtual void VisitReferenceToLocalVariable(LocalVariable variable)
        {
        }

        public virtual void VisitReferenceToMethodParameter(MethodParameter parameter)
        {
        }

        public virtual void VisitReferenceToTypeMember(TypeMember type)
        {
        }

        protected internal virtual void VisitAbstractStatement(AbstractStatement statement)
        {
        }

        protected internal virtual void VisitAbstractExpression(AbstractExpression expression)
        {
            if (expression.Type != null)
            {
                VisitReferenceToTypeMember(expression.Type);
            }
        }
    }
}
