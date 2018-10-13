using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Example.WebUIModel
{
    [ProgrammingModel]
    public class FormComponent<TModel> : AbstractComponent<TModel>
    {
        public override void Controller()
        {
        }

        public event Func<Task> Submitting;

        private void RaiseSubmitting()
        {
            Submitting?.Invoke();
        }
    }
}
