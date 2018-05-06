using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Example.WebUIModel
{
    [ProgrammingModel]
    public class FormComponent<TModel> : AbstractComponent
    {
        private void RaiseSubmitting()
        {
            Submitting?.Invoke();
        }

        public event Func<Task> Submitting;
    }
}
