using System;
using System.Collections.Generic;
using System.Text;
using Example.WebUIModel.Metadata;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace Example.HyperappAdapter.Components
{
    public class ModelMemberAccessRewriter : StatementRewriter
    {
        private readonly WebPageMetadata _metaPage;
        private readonly AbstractExpression _modelMemberReplacement;

        public ModelMemberAccessRewriter(WebPageMetadata metaPage, LocalVariable modelVariable)
            : this(metaPage, modelMemberReplacement: modelVariable.AsExpression())
        {
        }

        public ModelMemberAccessRewriter(WebPageMetadata metaPage, AbstractExpression modelMemberReplacement)
        {
            _metaPage = metaPage;
            _modelMemberReplacement = modelMemberReplacement;
        }

        public override AbstractExpression RewriteMemberExpression(MemberExpression expression)
        {
            if (IsModelPropertyMemberExpression(expression))
            {
                return _modelMemberReplacement;
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
