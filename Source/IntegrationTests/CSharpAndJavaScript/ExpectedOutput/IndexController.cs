using Microsoft.AspNetCore.Mvc;
using Example.App;
using Example.App.Services;

namespace Example.App.WebApiLayer
{
    [Route("api/[controller]")]
    public class IndexController : Controller
    {
        private readonly IGreetingService _greetingService;

        public IndexController(IGreetingService greetingService)
        {
            _greetingService = greetingService;
        }

        [HttpPost("getGreetingForName")]
        [InvalidModelAutoResponder]
        public async Task<JsonResult> Post([FromBody] GetGreetingForNameRequest requestData)
        {
            var resultValue = await _greetingService.GetGreetingForName(requestData.Name);
            return Json(resultValue);
        }

        public class GetGreetingForNameRequest
        {
            [Required, StringLength(50)]
            public string Name { get; set; }
        }
    }
}
