using System;
using System.Linq.Expressions;

namespace Example.WebUIModel
{
    public abstract class WebPage<TModel> : AbstractComponent<TModel>
    {
        //TODO: move to AbstractComponent
        protected TService GetBackendApiProxy<TService>()
            where TService : class
        {
            return null;
        }

        protected WebPage()
        {
        }

        protected WebPage(TModel initialModel)
        {
        }
    }
}
