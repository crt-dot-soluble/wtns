using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using WTNS.Cli;
using WTNS.Net.Models;

namespace WTNS.Net;

/// <summary>
/// Provides access to services which require authorization. (i.e. Logging in, Creating a user, Logging out)
/// </summary>
public static class AuthorizationProvider
{
    /// <summary>
    /// The <see cref="DbContext"/> object to use within Authorization Provider
    /// </summary>
    public static WTNSContext WTNSContext = new WTNSContext();

    /// <summary>
    /// Returns a <see cref="User"/> object if the username and password are valid, otherwise returns null.
    /// </summary>
    /// <param name="username">The name of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns></returns>
    public static User? Login(string username, string password)
    {
        var user = WTNSContext.Users.FirstOrDefault(u => u.UserName == username);

        if (user == null)
        {
            Repl.Log($"{username} does not exist", false, StructuredOutput.OutputMode.WARNING);
            Debug.Print($"{username} does not exist");
            return null;
        }
        else
        {
            if (HashingService.VerifyHash(password, user.Hash))
            {
                Repl.Log("Hash verified, returning the target user.");
                return user;
            }
            return null;
        }
    }

    /// <summary>
    /// Sets the session equal to null, requiring the user to reauthenticate to access a new session.
    /// </summary>
    public static void Logout() { }

    /// <summary>
    /// Creates a new user with the given username, password, and optional displayname and bio. Returns the new user if successful, otherwise returns null.
    /// </summary>
    /// <param name="username">The desired username.</param>
    /// <param name="password">The desired password.</param>
    /// <param name="displayname">The desired displayname</param>
    /// <param name="bio">The desired Bio</param>
    /// <returns>The user if successful, otherwise null.</returns>
    public static User? CreateUser(
        string username,
        string password,
        string? displayname = null,
        string? bio = null
    )
    {
        var existingUser = WTNSContext.Users.FirstOrDefault(u => u.UserName == username);
        if (existingUser != null)
        {
            Repl.Log($"{username} alreadys exists");
            Debug.Print($"{username} alreadys exists");
            return null;
        }

        var hashData = HashingService.CreateHash(password);
        var newUser = new User
        {
            UserName = username,
            DisplayName = displayname,
            Bio = bio,
            Hash = hashData.hashedPassword,
            Salt = hashData.salt,
            Active = 1,
            Session = new Session()
        };

        WTNSContext.Users.Add(newUser);

        try
        {
            Repl.Log($"Saving changes to database");
            WTNSContext.SaveChanges();
        }
        catch (Exception e)
        {
            Repl.Log(e.Message);
        }

        return newUser;
    }
}
