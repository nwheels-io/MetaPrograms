using System;
using System.Collections.Generic;
using System.Text;

namespace Example.WebUIModel
{
    public abstract class AbstractComponent
    {
        public abstract void Controller();
    }

    public abstract class AbstractComponent<TModel> : AbstractComponent
    {
        public TModel Model { get; set; }
    }
}
