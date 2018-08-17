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
                new JsxExpressionAttribute("data", GetBoundModelExpression(rootModel: @model)));

            foreach (var field in Metadata.ModelMetadata.Fields.Where(f => f.Direction == FieldDirection.Input))
            {
                rootElement.Add(new XElement(
                    "Form.field",
                    newScopeSelectorAttribute(),
                    new XAttribute("formId", formHtmlId),
                    new XAttribute("propName", field.Property.Name.ToString(CasingStyle.Camel))
                ));
            }

            rootElement.Add(new XElement("Form.submit",
                newScopeSelectorAttribute(),
                GetEventHandlerJsxExpression(@actions, nameof(Component.Submitting))));

            foreach (var field in Metadata.ModelMetadata.Fields.Where(f => f.Direction == FieldDirection.Output))
            {
                rootElement.Add(new XElement(
                    "Form.field",
                    newScopeSelectorAttribute(),
                    new XAttribute("formId", formHtmlId),
                    new XAttribute("propName", field.Property.Name.ToString(CasingStyle.Camel))
                ));
            }

            return rootElement;
        }
    }
}