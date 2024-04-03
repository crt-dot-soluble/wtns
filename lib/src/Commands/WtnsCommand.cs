using System.CommandLine;
using WTNS.Cli;
using WTNS.Net;

namespace WTNS.Commands;

public class WTNSCommand : RootCommand
{
    Option<bool> ExecuteOption = new Option<bool>(
        ["--execute", "-e"],
        "If this option is set to true the request will be processed without starting the REPL."
    );

    public WTNSCommand()
        : base("The WTNS root command.")
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
