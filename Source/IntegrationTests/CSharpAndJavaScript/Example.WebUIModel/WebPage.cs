using System;

namespace Example.WebUIModel
{
    public abstract class WebPage<TModel>
    {
        protected WebPage()
        {
        }

        protected WebPage(TModel initialModel)
        {
        }

        public TModel Model { get; }

        public abstract void Controller();

        protected TService GetBackendApiProxy<TService>()
            where TService : class
        {
            return null;
        }
    }
}
