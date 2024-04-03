using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using WTNS.Cli;

public class ReplCommand : Command
{
    public ReplCommand()
        : base("repl", "Run the REPL")
    {
        Handler = CommandHandler.Create(Repl.Instance.Listen);
    }
}
