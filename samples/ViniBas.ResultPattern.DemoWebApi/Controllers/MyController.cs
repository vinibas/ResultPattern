using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.DemoWebApi.Services;
using ViniBas.ResultPattern.AspNet;
using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.DemoWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MyController(IMyService myService) : ControllerBase
{
    private readonly IMyService _myService = myService;

    [HttpGet("{value}")]
    public IActionResult Get(int value)
        => _myService.Get(value).Match(Ok);

    [HttpPost("{value}")]
    public IActionResult Post(int value)
        => _myService.Post(value).Match(_ => Created());

    [HttpPut("{value}")]
    public IActionResult Custom(int value)
        => _myService.Custom(value).Match(Accepted);

    [HttpDelete("{success}")]
    public Result<string> Filter(bool success)
        => success ?
            Result<string>.Success("Deleted successfully") :
            Result<string>.Failure(Error.Failure("Err1", "Value must be true"));
}
