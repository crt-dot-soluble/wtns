using System.CommandLine;
using Wtns.Me.Lib.Cli;
using Wtns.Me.Lib.Net;

namespace Wtns.Me.Lib.Commands;

public class WtnsCommand : RootCommand
{
    Option<bool> ExecuteOption = new Option<bool>(
        ["--execute", "-e"],
        "If this option is set to true the request will be processed without starting the REPL."
    );

    public WtnsCommand()
        : base("The wtns root command.")
    {
        AddAlias("cli");
        AddCommand(new UserCommand());
        AddOption(ExecuteOption);
        this.SetHandler(
            (option) =>
            {
                if (option) { }
                else
                {
                    Repl.Instance.Listen();
                }
            },
            ExecuteOption
        );
    }
}
