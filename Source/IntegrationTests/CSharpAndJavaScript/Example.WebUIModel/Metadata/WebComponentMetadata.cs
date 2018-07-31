using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace Example.WebUIModel.Metadata
{
    public class WebComponentMetadata
    {
        private readonly ImperativeCodeModel _imperativeCodeModel;

        public WebComponentMetadata(
            WebPageMetadata page, 
            ImperativeCodeModel imperativeCodeModel, 
            PropertyMember declaredProperty, 
            MethodMember controllerMethod)
        {
            _imperativeCodeModel = imperativeCodeModel;

            this.Page = page;
            this.DeclaredProperty = declaredProperty;
            this.ComponentClass = declaredProperty.PropertyType;
            this.ModelClass = ComponentClass.GenericArguments.FirstOrDefault();
            this.EventMap = new EventMapMetadata();

            //TODO: add AbstractMember.HasAttribute<T>()/TryGetAttribute<T>()
            this.IsPredefined = (
                ComponentClass.Bindings.FirstOrDefault<Type>()?.GetCustomAttribute<ProgrammingModelAttribute>() != null);

            FindEventHandlers(controllerMethod);
        }

        public WebPageMetadata Page { get; }
        public PropertyMember DeclaredProperty { get; }
        public TypeMember ComponentClass { get; }
        public TypeMember ModelClass { get; }
        public bool IsPredefined { get; }
        public EventMapMetadata EventMap { get; }

        private void FindEventHandlers(MethodMember controllerMethod)
        {
            foreach (var statement in controllerMethod.Body.Statements)
            {
                if (TryParseEventHandlerAttachStatement(statement, out var eventName, out var handler))
                {
                    EventMap.AddHandler(eventName, handler);
                }
            }
        }

        private bool TryParseEventHandlerAttachStatement(AbstractStatement statement, out string eventName, out IFunctionContext handler)
        {
            eventName = null;
            handler = null;

            if (!(statement is ExpressionStatement expressionStatement) ||
                !(expressionStatement.Expression is AssignmentExpression assignment))
            {
                return false;
            }

            if (!(assignment.Left is MemberExpression eventMemberExpression) || 
                !(eventMemberExpression.Member is EventMember eventMember) || 
                !(eventMemberExpression.Target is MemberExpression componentMemberExpression) ||
                !(componentMemberExpression.Target is ThisExpression) || 
                componentMemberExpression.Member != this.DeclaredProperty)
            {
                return false;
            }

            if (!(assignment.Right is IFunctionContext function))
            {
                return false;
            }

            eventName = eventMember.Name;
            handler = function;
            return true;
        }
    }
}
