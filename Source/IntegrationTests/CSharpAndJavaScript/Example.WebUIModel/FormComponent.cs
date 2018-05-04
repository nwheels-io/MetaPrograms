using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Example.WebUIModel
{
    public class FormComponent<TModel> : AbstractComponent
    {
        public event Func<Task> Submitting;
    }
}
