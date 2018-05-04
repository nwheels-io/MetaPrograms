using Microsoft.AspNetCore.Mvc;
using Example.App;

namespace Example.App.WebApiLayer
{
    [Route("api")]
    public class ExampleAppController : Controller
    {
        private readonly IGreetingService _greetingService;

        public ExampleAppController(IGreetingService greetingService)
        {
            _greetingService = greetingService;
        }

        [HttpPost]
        public Task<string> GetGreetingForName(string name)
        {
            return _greetingService.GetGreetingForName(name);
        }
    }
}
