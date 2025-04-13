/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.DemoWebApi.Services;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.ActionResultMvc;

namespace ViniBas.ResultPattern.DemoWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MvcController(IMyService myService) : ControllerBase
{
    private readonly IMyService _myService = myService;

    /// <summary>
    /// You can pass a function with the return for the success case and another for the failure case.
    /// </summary>
    [HttpGet("{value}")]
    public IActionResult Get(int value)
        => _myService.Get(value).Match(Ok, Conflict);

    /// <summary>
    /// You can pass a conversion to ProblemDetails explicitly
    /// </summary>
    [HttpPost("{value}")]
    public IActionResult Post(int value)
        => _myService.Post(value).Match(_ => Created(), rr => rr.ToProblemDetailsActionResult());

    /// <summary>
    /// You can omit the failure parameter, which will return a ProblemDetails or a ObjectResult.
    /// In this case, we are also using a custom error type, NotAcceptable, mapped to Program.
    /// </summary>
    [HttpPut("{value}")]
    public IActionResult Custom(int value)
        => _myService.Custom(value).Match(Accepted);

    /// <summary>
    /// Or you can directly return a Result, and let the ActionFilter automatically convert 
    /// it to a ResultRespondeSuccess on success, or a ProblemDetails on error.
    /// </summary>
    [HttpDelete("{success}")]
    public Result<string> Filter(bool success)
        => success ?
            Result<string>.Success("Deleted successfully") :
            Result<string>.Failure(Error.Failure("Err1", "Value must be true"));
}
