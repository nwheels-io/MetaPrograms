using System.Collections.Generic;
using System.Xml.Linq;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;
using MetaPrograms.JavaScript.Writer;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Extensions;
using MetaPrograms.Members;
using static MetaPrograms.Fluent.Generator;

namespace Example.HyperappAdapter.Components
{
    public abstract class ComponentAdapter<TComponent> : IComponentAdapter
        where TComponent : AbstractComponent
    {
        public WebComponentMetadata Metadata { get; }
        public TComponent Component { get; }
        public AbstractExpression ModelBinding { get; }

        protected ComponentAdapter(WebComponentMetadata metadata, TComponent component)
        {
            this.Metadata = metadata;
            this.Component = component;
            this.ModelBinding = Metadata.PropertyMap.GetValueOrDefault("Model"); //TODO: find a way to use nameof()
        }

        public abstract void GenerateStateKeys();
        public abstract void GenerateActionKeys();
        public abstract XElement GenerateViewMarkup(AbstractExpression @model, AbstractExpression @actions);

        protected void GenerateEventHandlerActionKeys()
        {
            foreach (var eventName in Metadata.EventMap.GetHandledEventNames())
            {
                var handlerList = Metadata.EventMap.GetHandlers(eventName);

                if (handlerList.Count > 0)
                {
                    GenerateEventHandlerActionKey(eventName, handlerList);
                }
            }
        }

        protected void GenerateEventHandlerActionKey(IdentifierName eventName, IReadOnlyList<IFunctionContext> handlerList)
        {
            KEY(GetEventActionKeyName(eventName), LAMBDA(@model
                => DO.RETURN(ASYNC.LAMBDA((@state, @actions) => {

                    LOCAL("newModel", out var @newModel, USE("Object").DOT("assign").INVOKE(INITOBJECT(), @model));

                    var modelMemberAccessRewriter = new ModelMemberAccessRewriter(Metadata.Page, @newModel);
                    var apiMemberAccessRewriter = new BackendApiProxyAccessRewriter(Metadata.Page);

                    var actionBlock = CodeGeneratorContext.GetContextOrThrow().GetCurrentBlock();

                    foreach (var handler in handlerList)
                    {
                        var rewrittenHandlerBody = 
                            apiMemberAccessRewriter.RewriteBlockStatement(
                                modelMemberAccessRewriter.RewriteBlockStatement(handler.Body));

                        foreach (var statement in rewrittenHandlerBody.Statements)
                        {
                            actionBlock.AppendStatement(statement);
                        }
                    }

                    actions.DOT("replaceModel").INVOKE(@newModel);
                }))
            ));
        }

        protected string GetEventActionKeyName(IdentifierName eventName)
        {
            return Metadata.DeclaredProperty.Name.ToString(CasingStyle.Camel) + "_" + eventName.ToString(CasingStyle.Camel);
        }

        protected AbstractExpression GetBoundModelExpression(AbstractExpression rootModel)
        {
            if (ModelBinding != null)
            {
                var modelMemberAccessRewriter = new ModelMemberAccessRewriter(Metadata.Page, rootModel);
                return modelMemberAccessRewriter.RewriteExpression(ModelBinding);
            }

            return rootModel;
        }

        protected XAttribute GenerateEventHandlerAttribute(AbstractExpression @actions, IdentifierName eventName)
        {
            var handlers = Metadata.EventMap.GetHandlers(eventName);

            if (handlers.Count > 0)
            {
                var attributeName = eventName.AppendPrefixFragments("on").ToString(CasingStyle.Camel);
                var actionKey = GetEventActionKeyName(eventName);

                return new JsxExpressionAttribute(
                    attributeName,
                    LAMBDA(@data => DO.RETURN(@actions.DOT(actionKey).INVOKE(@data))));
            }

            return null;
        }

        protected JsxExpressionAttribute GenerateScopeSelectorAttribute() => new JsxExpressionAttribute(
            "scopeSelector",
            LAMBDA(@x => @x.DOT(Metadata.DeclaredProperty.Name)));
    }
}