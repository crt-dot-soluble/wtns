using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Wtns.Me.Lib.Net.Models;

/// <summary>
/// A thread-safe registry of users, keyed by their Id. Allows for a centralized management of users.
/// </summary>
public class UserRegistry
{
    // Using a thread-safe dictionary to store instances of User, keyed by their Id.
    private readonly ConcurrentDictionary<int, User> usersById =
        new ConcurrentDictionary<int, User>();

    // Singleton pattern implementation for the registry.
    private static readonly Lazy<UserRegistry> lazy = new Lazy<UserRegistry>(
        () => new UserRegistry()
    );


    /// <summary>
    /// The thread-safe 
    /// </summary>
    public static UserRegistry Instance => lazy.Value;

    // Private constructor to enforce the Singleton pattern.
    private UserRegistry() { }

    // Method to add or update a user in the registry.
    public void AddOrUpdateUser(User user)
    {
        usersById.AddOrUpdate(user.Id, user, (key, existingValue) => user);
    }

    // Method to retrieve a user by their unique Id.
    public User GetUserById(int userId)
    {
        if (usersById.TryGetValue(userId, out var user))
        {
            return user;
        }
        else
        {
            throw new KeyNotFoundException($"User with Id {userId} not found.");
        }
    }

    // Retrieves all users.
    public IEnumerable<User> GetAllUsers()
    {
        return usersById.Values;
    }

    // Removes a user by their unique Id.
    public bool RemoveUserById(int userId)
    {
        return usersById.TryRemove(userId, out _);
    }

    // Example of a method that might operate on the additional properties, like searching by username.
    public User GetUserByUserName(string userName)
    {
        foreach (var user in usersById.Values)
        {
            if (user.UserName == userName)
            {
                return user;
            }
        }
        throw new KeyNotFoundException($"User with UserName {userName} not found.");
    }
}
