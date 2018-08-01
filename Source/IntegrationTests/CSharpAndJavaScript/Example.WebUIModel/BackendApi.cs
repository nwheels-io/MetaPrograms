using System;
using System.Collections.Generic;
using System.Text;

namespace Example.WebUIModel
{
    public static class BackendApi
    {
        public static TService ProxyOf<TService>() 
            where TService : class 
            => null;
    }

    public class BackendApi<TService>
    {
        public TService Proxy { get; }
    }
}
