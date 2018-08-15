using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;
using MetaPrograms.Adapters.JavaScript.Writer;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

namespace Example.HyperappAdapter.Components
{
    public class FormComponentAdapter<TModel> : ComponentAdapter<FormComponent<TModel>>
    {
        public FormComponentAdapter(WebComponentMetadata metadata, FormComponent<TModel> component)
            : base(metadata, component)
        {
        }

        public override void GenerateStateKeys()
        {
            KEY(Metadata.DeclaredProperty.Name.ToString(CasingStyle.Camel), USE("Form").DOT("createState").INVOKE());
        }

        public override void GenerateActionKeys()
        {
            KEY(Metadata.DeclaredProperty.Name.ToString(CasingStyle.Camel), USE("Form").DOT("createActions").INVOKE());
            GenerateEventHandlerActionKeys();
        }

        public override XElement GenerateViewMarkup(AbstractExpression @model, AbstractExpression @actions)
        {
            JsxExpressionAttribute newScopeSelectorAttribute() => new JsxExpressionAttribute(
                "scopeSelector",
                LAMBDA(@x => @x.DOT(Metadata.DeclaredProperty.Name)));

            var formHtmlId = $"{Metadata.Page.PageClass.Name.ToString(CasingStyle.Camel)}_form";

            var rootElement = new XElement("Form.component",
                newScopeSelectorAttribute(),
                new XAttribute("id", formHtmlId),
                new JsxExpressionAttribute("data", @model));

            foreach (var inputProp in Metadata.ModelClass.Members.OfType<PropertyMember>())
            {
                rootElement.Add(new XElement(
                    "Form.field",
                    newScopeSelectorAttribute(),
                    new XAttribute("formId", formHtmlId),
                    new XAttribute("propName", inputProp.Name.ToString(CasingStyle.Camel))

                    
                ));
            }

            rootElement.Add(new XElement("Form.submit", newScopeSelectorAttribute()));

            return rootElement;
        }
    }
}