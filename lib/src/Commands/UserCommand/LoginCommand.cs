using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net;
using WTNS.Net;

namespace WTNS.Commands;

public class LoginCommand : Command
{
    public LoginCommand()
        : base("login", "Login to an existing account.")
    {
        AddArgument(new Argument<string>("username"));
        AddArgument(new Argument<string>("password"));

        Handler = CommandHandler.Create<string, string>(
            (username, password) =>
            {
                AuthorizationProvider.Login(username, password);
            }
        );
    }
}
