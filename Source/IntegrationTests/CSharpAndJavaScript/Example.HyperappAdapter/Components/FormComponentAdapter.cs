using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;
using MetaPrograms.Adapters.JavaScript;
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
            var formHtmlId = $"{Metadata.Page.PageClass.Name.ToString(CasingStyle.Camel)}_form";
            var rootElement = new XElement("Form.component",
                GenerateScopeSelectorAttribute(),
                new XAttribute("id", formHtmlId),
                new JsxExpressionAttribute("data", GetBoundModelExpression(rootModel: @model)));

            foreach (var field in Metadata.ModelMetadata.Fields.Where(f => f.Direction == FieldDirection.Input))
            {
                rootElement.Add(GenerateFieldMarkup(formHtmlId, field));
            }

            rootElement.Add(new XElement("Form.submit",
                GenerateScopeSelectorAttribute(),
                GenerateEventHandlerAttribute(@actions, nameof(Component.Submitting))));

            foreach (var field in Metadata.ModelMetadata.Fields.Where(f => f.Direction == FieldDirection.Output))
            {
                rootElement.Add(GenerateFieldMarkup(formHtmlId, field));
            }

            return rootElement;
        }

        private XElement GenerateFieldMarkup(string formHtmlId, ModelFieldMetadata field)
        {
            var element = new XElement(
                "Form.field",
                GenerateScopeSelectorAttribute(),
                new XAttribute("formId", formHtmlId),
                new XAttribute("propName", field.Property.Name.ToString(CasingStyle.Camel)));

            if (field.Label != null)
            {
                var labelText = new IdentifierName(field.Label, LanguageInfo.Entries.JavaScript(), style: CasingStyle.Pascal);
                element.Add(new XAttribute("label", labelText.ToString(CasingStyle.UserFriendly) + ":"));
            }

            element.Add(new JsxExpressionAttribute(
                "getter", 
                LAMBDA(@m => IIF(@m, whenTrue: @m.DOT(field.Property), whenFalse: ANY("???")))));

            if (field.Direction == FieldDirection.Input)
            {
                element.Add(new JsxExpressionAttribute(
                    "setter",
                    LAMBDA((@m, @v) => @m.DOT(field.Property).ASSIGN(@v))));
            }

            element.Add(new JsxExpressionAttribute("validation", GenerateValidationObject(field)));

            return element;
        }

        private AbstractExpression GenerateValidationObject(ModelFieldMetadata field)
        {
            return INITOBJECT(() => {
                if (field.Direction == FieldDirection.Output)
                {
                    KEY("isOutput", ANY(true));
                }

                if (field.IsRequired)
                {
                    KEY("required", ANY(true));
                }

                if (field.MaxLength.HasValue)
                {
                    KEY("maxLength", ANY(field.MaxLength.Value));
                }
            });
        }
    }
}