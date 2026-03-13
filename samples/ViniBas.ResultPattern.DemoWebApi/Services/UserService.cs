/*
 * Copyright (c) Vinícius Bastos da Silva 2026
 * This file is part of ResultPattern.
 * Licensed under the GNU Lesser General Public License v3 (LGPL v3).
 * See the LICENSE file in the project root for full details.
*/

using ViniBas.ResultPattern.ResultObjects;

namespace ViniBas.ResultPattern.DemoWebApi.Services;

public record UserModel(string Name, int Age);

public interface IUserService
{
    Result Health(bool alive);
    Result<IEnumerable<UserModel>> GetAllUsers();
    Result<UserModel> GetUserByName(string name);
    Result SaveNewUser(UserModel user);
    Result<UserModel> UpdateUser(UserModel user);
    Result HardDeleteUser(string name);
}

public class UserService : IUserService
{
    private readonly List<UserModel> _users = new List<UserModel>();

    public Result Health(bool alive)
        => alive ? Result.Success() : Result.Failure(Error.Failure(string.Empty, "The service is not alive"));

    public Result SaveNewUser(UserModel user)
    {
        var validationErrors = new List<Error>();

        if (string.IsNullOrWhiteSpace(user.Name))
            validationErrors.Add(Error.Validation("Err1", "Name cannot be empty"));

        if (user.Age < 18)
            validationErrors.Add(Error.Validation("Err2", "Age must be greater than or equal to 18"));

        if (validationErrors.Any())
            return validationErrors;

        if (_users.Any(u => u.Name == user.Name))
            return Error.Conflict("Err3", "User already exists");

        _users.Add(user);
        return Result.Success();
        // Less semantic, but also possible:
        // return Error.None;
    }

    public Result<UserModel> UpdateUser(UserModel user)
    {
        var validationErrors = new List<Error>();

        if (string.IsNullOrWhiteSpace(user.Name))
            validationErrors.Add(Error.Validation("Err1", "Name cannot be empty"));

        if (user.Age < 18)
            validationErrors.Add(Error.Validation("Err2", "Age must be greater than or equal to 18"));

        if (validationErrors.Any())
            return validationErrors;

        var userIndex = _users.FindIndex(u => u.Name == user.Name);

        if (userIndex == -1)
            return Error.NotFound("Err4", "User not found");

        _users[userIndex] = user;
        return Result.Success(user);
    }

    public Result<IEnumerable<UserModel>> GetAllUsers()
        => _users;

    public Result<UserModel> GetUserByName(string name)
    {
        var user = _users.FirstOrDefault(u => u.Name == name);

        if (user == null)
            return Error.NotFound("Err4", "User not found");

        // It can be returned explicitly, like:
        // return Result<UserModel>.Success(user);
        // Or more directly:
        return user;
    }

    public Result HardDeleteUser(string name)
        => new Error("Err5", "The user cannot be deleted", "NotAcceptable");
}
