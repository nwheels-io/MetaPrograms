using System;
using System.Collections.Generic;
using System.Text;

namespace Example.WebUIModel
{
    public class BackendApi<TService>
    {
        public TService Proxy { get; }
    }
}
