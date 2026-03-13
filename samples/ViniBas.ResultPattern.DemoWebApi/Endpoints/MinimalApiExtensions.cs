/*
 * Copyright (c) Vinícius Bastos da Silva 2025
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViniBas.ResultPattern.AspNet.Configurations;
using ViniBas.ResultPattern.AspNet.MinimalApi;
using ViniBas.ResultPattern.DemoWebApi.Services;
using ViniBas.ResultPattern.ResultObjects;
using ViniBas.ResultPattern.ResultResponses;

namespace ViniBas.ResultPattern.DemoWebApi.Endpoints;

using MultipleResultsOnMapPut = Results<
    Results<Ok<ResultResponseSuccess<UserModel>>, JsonHttpResult<ResultResponseError>>,
    Results<Ok<ResultResponseSuccess<UserModel>>, ProblemHttpResult>,
    Results<Ok<UserModel>, JsonHttpResult<IEnumerable<ErrorDetails>>>,
    Results<Ok<UserModel>, ProblemHttpResult>>;

public static class MinimalApiExtensions
{
    private static readonly IUserService _userService = new UserService();

    public static void RegisterUserGenericEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("minimal-generic")
            .WithTags("MinimalApiGeneric");

        group
            .MapGet("health/{alive}",
                (bool alive) => _userService.Health(alive).Match(Results.Ok, Results.BadRequest))
            .WithDescription("You can pass a function with the return for the success" +
                "case and another for the failure case.");

        group
            .MapGet("{name}",
                (string name) => _userService.GetUserByName(name).Match(r => Results.Ok(r.Data)))
            .WithDescription("You can omit one of the parameters. For example, omit onFailure " +
                "to let the library return an IActionResult based on the type of error returned.");

        group
            .MapPost("",
                (UserModel user) => _userService.SaveNewUser(user).Match())
            .WithDescription("You can omit both parameters. You will receive an Ok200 status" +
                "in case of success, or a status based on the error type in case of failure.");

        group
            .MapPut("", (UserModel user, bool useProblemDetails, bool unwrapSuccessData) =>
            {
                using (ScopedConfiguration.Override(
                    useProblemDetails: useProblemDetails,
                    unwrapSuccessData: unwrapSuccessData))
                {
                    return _userService.UpdateUser(user).Match();
                }
            })
            .WithDescription("You can also locally override some of the global settings.");

        group
            .MapDelete("{userName}", (string userName) => _userService.HardDeleteUser(userName))
            .WithResponseMappingFilter()
            .WithDescription("You can also return a custom error, simply by registering it in " +
                "GlobalConfiguration.ErrorTypeMaps. In fact, if you have registered the " +
                "ResponseMappingFilter filter, you can return the Result directly, which the " +
                "filter will convert to a suitable IActionResult.");
    }

    public static void RegisterUserUnionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("minimal-union")
            .WithTags("MinimalApiUnionTypes");

        group
            .MapGet("health/{alive}", (bool alive) => _userService.Health(alive)
                    .MatchResults<Ok<ResultResponseSuccess>, BadRequest<ResultResponseError>>
                        (r => TypedResults.Ok(r), r => TypedResults.BadRequest(r)))
            .WithDescription("You can pass a function with the return for the success" +
                "case and another for the failure case.");

        group
            .MapGet("{name}",
                (string name) => _userService.GetUserByName(name)
                    .MatchResults<Ok<UserModel>, JsonHttpResult<ResultResponseError>, UserModel>
                    (r => TypedResults.Ok(r.Data)))
            .WithDescription("You can omit one of the parameters. For example, omit onFailure " +
                "to let the library return an IActionResult based on the type of error returned.");

        group
            .MapPost("",
                (UserModel user) => _userService.SaveNewUser(user)
                    .MatchResults<Ok<ResultResponseSuccess>, JsonHttpResult<ResultResponseError>>())
            .WithDescription("You can omit both parameters. You will receive an Ok200 status" +
                "in case of success, or a status based on the error type in case of failure.");

        group
            .MapPut("", (UserModel user, [FromQuery] bool useProblemDetails, [FromQuery] bool unwrapSuccessData) =>
            {
                var updateResult = _userService.UpdateUser(user);

                using (ScopedConfiguration.Override(
                    useProblemDetails: useProblemDetails,
                    unwrapSuccessData: unwrapSuccessData))
                {
                    return (useProblemDetails, unwrapSuccessData) switch
                    {
                        (false, false) => (MultipleResultsOnMapPut)
                            updateResult.MatchResults<Ok<ResultResponseSuccess<UserModel>>, JsonHttpResult<ResultResponseError>, UserModel>(),
                        (true, false) => (MultipleResultsOnMapPut)
                            updateResult.MatchResults<Ok<ResultResponseSuccess<UserModel>>, ProblemHttpResult, UserModel>(),
                        (false, true) => (MultipleResultsOnMapPut)
                            updateResult.MatchResults<Ok<UserModel>, JsonHttpResult<IEnumerable<ErrorDetails>>, UserModel>(),
                        (true, true) => (MultipleResultsOnMapPut)
                            updateResult.MatchResults<Ok<UserModel>, ProblemHttpResult, UserModel>(),
                    };
                }
            })
            .WithDescription("You can also locally override some of the global settings.");

        group
            .MapDelete("{userName}", (string userName) => _userService.HardDeleteUser(userName))
            .WithResponseMappingFilter()
            .WithDescription("You can also return a custom error, simply by registering it in " +
                "GlobalConfiguration.ErrorTypeMaps. In fact, if you have registered the " +
                "ResponseMappingFilter filter, you can return the Result directly, which the " +
                "filter will convert to a suitable IActionResult.");
    }
}
