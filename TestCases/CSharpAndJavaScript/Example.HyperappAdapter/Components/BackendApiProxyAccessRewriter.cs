using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Example.HyperappAdapter.Metadata;
using Example.WebUIModel.Metadata;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace Example.HyperappAdapter.Components
{
    public class BackendApiProxyAccessRewriter : StatementRewriter
    {
        private readonly WebPageMetadata _metaPage;
        private readonly HyperappPageMetadata _metaPageExtension;
        private readonly HashSet<TypeMember> _backendApiTypes;

        public BackendApiProxyAccessRewriter(WebPageMetadata metaPage)
        {
            _metaPage = metaPage;
            _metaPageExtension = _metaPage.Extensions.Get<HyperappPageMetadata>();
            _backendApiTypes = new HashSet<TypeMember>(metaPage.BackendApis.Select(api => api.InterfaceType));
        }

        public override AbstractExpression RewriteMemberExpression(MemberExpression expression)
        {
            if (IsApiPropertyMemberExpression(expression, out var serviceType))
            {
                return new LocalVariableExpression {
                    Bindings = new BindingCollection(expression.Bindings),
                    Variable = _metaPageExtension.ServiceVarByType[serviceType]
                };
            }

            return base.RewriteMemberExpression(expression);
        }

        private bool IsApiPropertyMemberExpression(MemberExpression expression, out TypeMember serviceType)
        {
            if (expression.Target is ThisExpression && 
                expression.Member is PropertyMember property && 
                _backendApiTypes.Contains(property.PropertyType))
            {
                serviceType = property.PropertyType;
                return true;
            }

            serviceType = null;
            return false;
        }
    }
}
