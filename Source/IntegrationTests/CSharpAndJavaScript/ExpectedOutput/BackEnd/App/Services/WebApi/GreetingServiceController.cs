using System;
using System.Threading.Tasks;
using Example.App.Services;
using Example.AspNetAdapter;
using Microsoft.AspNetCore.Mvc;

namespace Example.App.Services.WebApi
{
    [Route("api/[controller]")]
    public class GreetingServiceController : Controller
    {
        private readonly IGreetingService _service;
        public GreetingServiceController(IGreetingService service)
        {
            this._service = service;
        }

        [InvalidModelAutoResponder, HttpPost("getGreetingForName")]
        public async Task<IActionResult> GetGreetingForName([FromBody] GetGreetingForNameInvocation requestData)
        {
            string resultValue;
            resultValue = await this._service.GetGreetingForName(requestData.Name);
            return this.Json(resultValue);
        }
    }
}