using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Wtns.Me.Lib.Cli;

public class ReplCommand : Command
{
    public ReplCommand()
        : base("repl", "Run the REPL")
    {
        Handler = CommandHandler.Create(Repl.Instance.Listen);
    }
}
