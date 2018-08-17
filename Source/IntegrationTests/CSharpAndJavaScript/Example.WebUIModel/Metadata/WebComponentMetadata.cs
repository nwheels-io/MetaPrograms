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
            this.PropertyMap = new Dictionary<IdentifierName, AbstractExpression>();
            this.EventMap = new EventMapMetadata();
            this.ModelMetadata = new ModelObjectMetadata(ModelClass);

            //TODO: add AbstractMember.HasAttribute<T>()/TryGetAttribute<T>()
            this.IsPredefined = (
                ComponentClass.Bindings.FirstOrDefault<Type>()?.GetCustomAttribute<ProgrammingModelAttribute>() != null);

            ParsePropertyValues();
            FindEventHandlers(controllerMethod);
        }

        private void ParsePropertyValues()
        {
            if (DeclaredProperty.Getter.Body.Statements.FirstOrDefault() is ReturnStatement @return)
            {
                if (@return.Expression is NewObjectExpression newObj && newObj.Initializer != null)
                {
                    foreach (var propValue in newObj.Initializer.PropertyValues)
                    {
                        PropertyMap.Add(propValue.Name, propValue.Value);
                    }
                }
            }
        }

        public WebPageMetadata Page { get; }
        public PropertyMember DeclaredProperty { get; }
        public TypeMember ComponentClass { get; }
        public TypeMember ModelClass { get; }
        public ModelObjectMetadata ModelMetadata { get; }
        public bool IsPredefined { get; }
        public Dictionary<IdentifierName, AbstractExpression> PropertyMap { get; }
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

        private bool TryParseEventHandlerAttachStatement(AbstractStatement statement, out IdentifierName eventName, out IFunctionContext handler)
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
