/*
 * Copyright (c) Vinícius Bastos da Silva 2025-2026
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
    Results<Ok<ResultResponseSuccess<UserModel>>, BadRequest<ResultResponseError>, NotFound<ResultResponseError>>,
    Results<Ok<ResultResponseSuccess<UserModel>>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>,
    Results<Ok<UserModel>, BadRequest<IEnumerable<ErrorDetails>>, NotFound<IEnumerable<ErrorDetails>>>,
    Results<Ok<UserModel>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>>;

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
            .WithDescription("You can pass a function to handle the success " +
                "case and another to handle the failure case.");

        group
            .MapGet("{name}",
                (string name) => _userService.GetUserByName(name).Match(r => Results.Ok(r.Data)))
            .WithDescription("You can omit one of the parameters. For example, omit onFailure " +
                "to let the library return an IResult based on the type of error returned.");

        group
            .MapPost("",
                (UserModel user) =>
                {
                    // Override UseProblemDetails locally to choose between returning ResultResponseError or ProblemDetails.
                    using (ScopedConfiguration.Override(useProblemDetails: false))
                    {
                        return _userService.SaveNewUser(user).Match();
                    }
                })
            .WithDescription("You can omit both parameters. You will receive an Ok200 status " +
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
                "filter will convert to a suitable IResult.");
    }

    public static void RegisterUserUnionEndpoints(this IEndpointRouteBuilder routes, bool isProduction)
    {
        var group = routes
            .MapGroup("minimal-union")
            .WithTags("MinimalApiUnionTypes");

        if (isProduction)
            group.AddEndpointFilter<TypedResultCastFallbackFilter>();

        group
            .MapGet("health/{alive}", (bool alive) => _userService.Health(alive)
                    .MatchResults<Ok<ResultResponseSuccess>, BadRequest<ResultResponseError>>
                        (r => TypedResults.Ok(r), r => TypedResults.BadRequest(r)))
            .WithDescription("You can pass a function to handle the success " +
                "case and another to handle the failure case.");

        group
            .MapGet("{name}",
                (string name) => _userService.GetUserByName(name)
                    .MatchResults<Ok<UserModel>, NotFound<ProblemDetails>, UserModel>
                    (r => TypedResults.Ok(r.Data)))
            .WithDescription("You can omit one of the parameters. For example, omit onFailure " +
                "to let the library return a typed result based on the type of error returned.");

        group
            .MapPost("",
                (UserModel user) =>
                {
                    // Override UseProblemDetails locally: this changes the typed return between
                    // ResultResponseError (false) and ProblemDetails (true) in the error type parameters.
                    using (ScopedConfiguration.Override(useProblemDetails: false))
                    {
                        return _userService.SaveNewUser(user)
                        .MatchResults<
                            Ok<ResultResponseSuccess>,
                            BadRequest<ResultResponseError>,
                            Conflict<ResultResponseError>>();
                    }
                }).WithDescription("You can omit both parameters. You will receive an Ok200 status " +
                "in case of success, or a status based on the error type in case of failure.");

        group
            .MapPut("", (UserModel user, bool useProblemDetails, bool unwrapSuccessData) =>
            {
                var updateResult = _userService.UpdateUser(user);

                using (ScopedConfiguration.Override(
                    useProblemDetails: useProblemDetails,
                    unwrapSuccessData: unwrapSuccessData))
                {
                    return (useProblemDetails, unwrapSuccessData) switch
                    {
                        (false, false) => (MultipleResultsOnMapPut)
                            updateResult.MatchResults<Ok<ResultResponseSuccess<UserModel>>, BadRequest<ResultResponseError>, NotFound<ResultResponseError>, UserModel>(),
                        (true, false) => (MultipleResultsOnMapPut)
                            updateResult.MatchResults<Ok<ResultResponseSuccess<UserModel>>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>, UserModel>(),
                        (false, true) => (MultipleResultsOnMapPut)
                            updateResult.MatchResults<Ok<UserModel>, BadRequest<IEnumerable<ErrorDetails>>, NotFound<IEnumerable<ErrorDetails>>, UserModel>(),
                        (true, true) => (MultipleResultsOnMapPut)
                            updateResult.MatchResults<Ok<UserModel>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>, UserModel>(),
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
                "filter will convert to a suitable IResult.");
    }
}
