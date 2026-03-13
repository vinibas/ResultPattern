/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.DemoWebApi.Services;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.AspNet.Mvc;
using ViniBas.ResultPattern.AspNet.Configurations;

namespace ViniBas.ResultPattern.DemoWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MvcController : ControllerBase
{
    private static readonly IUserService _userService = new UserService();

    /// <summary>
    /// You can pass a function with the return for the success case and another for the failure case.
    /// </summary>
    [HttpGet("health/{alive}")]
    public IActionResult Health(bool alive)
        => _userService.Health(alive).Match(Ok, BadRequest);

    /// <summary>
    /// You can omit one of the parameters. For example, omit onFailure to let the
    /// library return an IActionResult based on the type of error returned.
    /// </summary>
    [HttpGet("{name}")]
    public IActionResult Get(string name)
        => _userService.GetUserByName(name).Match(r => Ok(r.Data));

    /// <summary>
    /// You can omit both parameters. You will receive an Ok200 status
    /// in case of success, or a status based on the error type in case of failure.
    /// </summary>
    [HttpPost]
    public IActionResult CreateNewUser(UserModel user)
        => _userService.SaveNewUser(user).Match();

    /// <summary>
    /// You can also locally override some of the global settings.
    /// </summary>
    [HttpPut]
    public IActionResult UpdateUser(
        [FromBody] UserModel user, [FromQuery] bool useProblemDetails, [FromQuery] bool unwrapSuccessData)
    {
        using (ScopedConfiguration.Override(
            useProblemDetails: useProblemDetails,
            unwrapSuccessData: unwrapSuccessData))
        {
            return _userService.UpdateUser(user).Match();
        }
    }

    /// <summary>
    /// You can also return a custom error, simply by registering it in GlobalConfiguration.ErrorTypeMaps.
    /// In fact, if you have registered the ResponseMappingFilter filter, you can return
    /// the Result directly, which the filter will convert to a suitable IActionResult.
    /// </summary>
    [HttpDelete("{userName}")]
    public Result Delete(string userName)
        => _userService.HardDeleteUser(userName);
}
