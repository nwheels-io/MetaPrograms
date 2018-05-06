using Example.App.Services;
using Example.WebUIModel;

namespace Example.App
{
    [WebUI.Semantic.IndexPage]
    public class HelloPage : WebPage<HelloPage.HelloModel>
    {
        public BackendApi<IGreetingService> GreetingService { get; }
        public FormComponent<HelloModel> Form { get; }

        public override void Controller()
        {
            Form.Submitting += async () => {
                Model.Greeting = await GreetingService.Proxy.GetGreetingForName(Model.Name);
            };
        }

        public class HelloModel
        {
            [PropertyContract.Semantic.Input]
            [PropertyContract.Validation.Required(AllowEmpty = false, MaxLength = 50)]
            public string Name { get; set; }

            [PropertyContract.Semantic.Output]
            [PropertyContract.I18n.Label("SystemSaid")]
            public string Greeting { get; set; }
        }
    }
}
