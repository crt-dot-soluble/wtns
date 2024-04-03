using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using WTNS.Net;

public class CreateCommand : Command
{
    public CreateCommand()
        : base("create", "Create a new user account.")
    {
        AddArgument(new Argument<string>("username", "The username of the new user."));
        AddArgument(new Argument<string>("password", "The password of the new user."));

        Handler = CommandHandler.Create<string, string>(
            (username, password) =>
            {
                AuthorizationProvider.CreateUser(username, password);
            }
        );
    }
}
