using System;
using System.Collections.Generic;
using System.Text;
using Example.WebUIModel.Metadata;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.HyperappAdapter.Components
{
    public class ModelMemberAccessRewriter : StatementRewriter
    {
        private readonly WebPageMetadata _metaPage;
        private readonly LocalVariable _modelVariable;

        public ModelMemberAccessRewriter(WebPageMetadata metaPage, LocalVariable modelVariable)
        {
            _metaPage = metaPage;
            _modelVariable = modelVariable;
        }

        public override AbstractExpression RewriteMemberExpression(MemberExpression expression)
        {
            if (IsModelPropertyMemberExpression(expression))
            {
                return new LocalVariableExpression {
                    Bindings = new BindingCollection(expression.Bindings),
                    Variable = _modelVariable
                };
            }

            return base.RewriteMemberExpression(expression);
        }

        private bool IsModelPropertyMemberExpression(MemberExpression expression)
        {
            if (expression.Member == _metaPage.ModelProperty && expression.Target is ThisExpression)
            {
                return true;
            }

            return false;
        }
    }
}
