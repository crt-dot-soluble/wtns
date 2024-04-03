using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Net;
using System.Reflection.Metadata;
using Microsoft.Win32.SafeHandles;
using WTNS.Cli;
using WTNS.Net;
using WTNS.Net.Models;

namespace WTNS.Commands;

/// <summary>
/// User command which exposes the built
/// </summary>
public class UserCommand : Command
{
    /// <summary>
    /// Contructor for <see cref="UserCommand"/>.
    /// </summary>
    public UserCommand()
        : base("user", "Allows for user features such as creation, deactivation and login.")
    {
        AddCommand(new LoginCommand());
        AddCommand(new CreateCommand());
        // Argument<string> requestArgument = new Argument<string>()
        // {
        //     Name = "request",
        //     Description = "A string value representing the type of request.\nCurrent options are ",
        //     Arity = ArgumentArity.ExactlyOne
        // };

        // requestArgument.SetDefaultValue("login");

        // AddArgument(requestArgument);

        // Handler = CommandHandler.Create<string>(
        //     (request) =>
        //     {
        //         switch (request.ToLower())
        //         {
        //             case "login":
        //                 HandleLoginArgument();
        //                 break;
        //             case "create":
        //                 Repl.Log($"create argument set.");
        //                 break;

        //             case "deactivate":
        //                 Repl.Log($"deactivate argument set.");
        //                 break;
        //         }
        //     },
        //     requestArgument
        // );

        //this.SetHandler(HandleLoginArgument, requestArgument);
    }

    private void HandleLoginArgument(string option)
    {
        switch (option.ToLower())
        {
            case "login":
                var login = AuthorizationProvider.Login("WTNS", "WTNS");
                if (login != null)
                {
                    UserRegistry.Instance.AddOrUpdateUser(login);
                }
                break;
            case "create":
                Repl.Log($"create argument set.");
                break;

            case "deactivate":
                Repl.Log($"deactivate argument set.");
                break;
        }
    }
}
