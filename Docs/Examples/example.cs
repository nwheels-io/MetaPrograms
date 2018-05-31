//-------------------------------------------------------
// An example of class to be generated - 26 lines
//-------------------------------------------------------

[Route("api/[controller]")]
public class MyController : Controller
{
    private readonly IService _service;
    private readonly ILogger _logger;

    public MyController(IService service, ILogger logger)
    {
        _service = service;
        _logger = logger;
    } 

    [HttpPost]
    IActionResult DoSomething([FromBody] string value) 
    {
        try
        {
            _service.DoSomething(value);
            return this.Ok();
        }
        catch (Exception e)
        { 
            _logger.Error($"{e.GetType().Name}: {e.Message}");
            return this.NotFound();
        }
    }
}
