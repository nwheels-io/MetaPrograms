using System.Collections.Generic;
using System.Xml.Linq;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

namespace Example.HyperappAdapter.Components
{
    public abstract class ComponentAdapter<TComponent> : IComponentAdapter
        where TComponent : AbstractComponent
    {
        public WebComponentMetadata Metadata { get; }
        public TComponent Component { get; }

        protected ComponentAdapter(WebComponentMetadata metadata, TComponent component)
        {
            this.Metadata = metadata;
            this.Component = component;
        }

        public abstract void GenerateStateKeys();
        public abstract void GenerateActionKeys();
        public abstract XElement GenerateViewMarkup();

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

        protected void GenerateEventHandlerActionKey(string eventName, IReadOnlyList<IFunctionContext> handlerList)
        {
            KEY($"{Metadata.DeclaredProperty.Name}_{eventName}", LAMBDA(@model
                => DO.RETURN(LAMBDA((@state, @actions) => {

                    LOCAL("newModel", out var @newModel, USE("Object").DOT("assign").INVOKE(INITOBJECT(), @model));

                    var modelMemberAccessRewriter = new ModelMemberAccessRewriter(Metadata.Page, @newModel);
                    var actionBlock = CodeGeneratorContext.GetContextOrThrow().GetCurrentBlock();

                    foreach (var handler in handlerList)
                    {
                        var rewrittenHandlerBody = modelMemberAccessRewriter.RewriteBlockStatement(handler.Body);

                        foreach (var statement in rewrittenHandlerBody.Statements)
                        {
                            actionBlock.AppendStatement(statement);
                        }
                    }

                    actions.DOT("replaceModel").INVOKE(@newModel);
                }))
            ));
        }
    }
}