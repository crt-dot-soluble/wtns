using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WTNS.Cli;

namespace WTNS.Commands
{
    /// <summary>
    /// A registry which exposes commands to the CLI.
    /// All commands must be registered before they can be discovered by the CLI.
    /// For help creating commands please see <see cref="Command"/>.
    /// Follows singleton design pattern with support for lazy initialization.
    /// </summary>
    public sealed class CommandRegistry
    {
        private static readonly Lazy<CommandRegistry> _lazyInstance = new Lazy<CommandRegistry>(
            () => new CommandRegistry()
        );

        private readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>(
            StringComparer.OrdinalIgnoreCase
        );

        private List<string> _history = new List<string>();

        private readonly Command _rootCommand = new WTNSCommand();

        private Parser _parser = new CommandLineBuilder(new WTNSCommand()).UseDefaults().Build();

        private CommandRegistry()
        {
            RegisterCommand(new WTNSCommand());
            RegisterCommand(new UserCommand());
            RegisterCommand(new ReplCommand());
        }

        /// <summary>
        /// Represents the singleton instance of the <see cref="CommandRegistry"/> class.
        /// </summary>
        public static CommandRegistry Instance => _lazyInstance.Value;

        /// <summary>
        /// Registers a <see cref="Command"/> with the registry.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to register.</param>
        /// <returns>True of the <see cref="Command"/> was registered, false otherwise.</returns>
        public bool RegisterCommand(Command command)
        {
            if (!_commands.ContainsKey(command.Name))
            {
                _commands[command.Name] = command;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Registers a <see cref="RootCommand"/> with the registry.
        /// </summary>
        /// <param name="rootCommand">The <see cref="RootCommand"/> to register.</param>
        /// <returns>True of the <see cref="RootCommand"/> was registered, false otherwise.</returns>
        public bool RegisterCommand(RootCommand rootCommand)
        {
            if (!_commands.ContainsKey(rootCommand.Name))
            {
                _commands[rootCommand.Name] = rootCommand;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a <see cref="Command"/> from the registry.
        /// </summary>
        /// <param name="commandName">The name of the <see cref="Command"/> to remove.</param>
        /// <returns>True if the <see cref="Command"/> was found and removed, false otherwise.</returns>
        public bool RemoveCommand(string commandName)
        {
            return _commands.Remove(commandName);
        }

        /// <summary>
        /// Gets a <see cref="Command"/> from the registry.
        /// </summary>
        /// <param name="commandName">The name of the <see cref="Command"/> to get.</param>
        /// <returns>The <see cref="Command"/> if found, null otherwise.</returns>
        public Command? GetCommand(string commandName)
        {
            if (_commands.TryGetValue(commandName, out var command))
            {
                return command;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a dictionary of all registered <see cref="Command"/>s.
        /// </summary>
        /// <returns>A dictionary of all available commands.</returns>
        public IEnumerable<Command> GetRegisteredCommands()
        {
            return _commands.Values;
        }

        /// <summary>
        /// Executes a <see cref="Command"/> from the registry.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to parse and execute.</param>
        public void ExecuteCommand(string command, bool isReplay = false)
        {
            if (!isReplay)
            {
                _history.Add(command);
            }

            string[] args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length > 0 && _commands.TryGetValue(args[0], out Command cmd))
            {
                // var rc = (rootCommand == null) ? _rootCommand : new WTNSCommand();
                cmd.Invoke(args);
            }
            else
            {
                Repl.Log($"Command not found.", false, StructuredOutput.OutputMode.ERROR);
            }
        }

        /// <summary>
        /// Executes a <see cref="Command"/> from the registry.
        /// </summary>
        /// <param name="command">The array of strings that represents the commands.</param>
        public void ExecuteCommand(string[] command, bool isReplay = false)
        {
            string fullCommand = string.Join(" ", command);
            if (!isReplay)
            {
                _history.Add(fullCommand);
            }

            if (command.Length > 0 && _commands.TryGetValue(command[0], out Command cmd))
            {
                // var rc = (_rootCommand == null) ? _rootCommand : new WTNSCommand();
                // if (_parser.Parse(command).Errors.Count != 0)
                // {
                //     Debug.Print($"CommandRegistry: Failed to parse the command: {fullCommand}");
                //     return;
                // }

                Debug.Print($"CommandRegistry: Parsed the command: {fullCommand}");
                cmd.Invoke(command);

                //Repl.Log($"{fullCommand}");
                //cmd.Invoke(command);
                //Repl.Log($"{fullCommand}", true);
            }
            else
            {
                Console.WriteLine("Command not found.");
            }
        }

        /// <summary>
        /// Returns a list of all <see cref="Command">s that have been executed.
        /// </summary>
        /// <returns>A list of all executed <see cref="Command"/>s.</returns>
        public IEnumerable<string> GetHistory()
        {
            return _history;
        }

        /// <summary>
        /// Clears the history of executed <see cref="Command"/>s.
        /// </summary>
        public void ClearHistory()
        {
            _history.Clear();
        }

        /// <summary>
        /// Saves the history of executed <see cref="Command"/>s to a file.
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveHistory(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string json = JsonSerializer.Serialize(_history, options);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads the history of executed <see cref="Command"/>s from a file.
        /// </summary>
        /// <param name="filePath">The path to load data from - Will be moved ot Configuration later.</param>
        public List<string>? LoadHistory(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var historyData = JsonSerializer.Deserialize<List<string>>(json);
                if (historyData != null)
                {
                    // Don't clear history unless the historyData is valid
                    _history.Clear();
                    _history.AddRange(historyData);
                    Repl.Log(
                        $"Loaded {_history.Count} commands from history file.",
                        false,
                        StructuredOutput.OutputMode.WARNING
                    );
                    return historyData;
                }

                return null;
            }
            return null;
        }

        /// <summary>
        /// Replays the history of executed <see cref="Command"/>s.
        /// </summary>
        public void ReplayHistory()
        {
            Repl.Log("[BEGIN REPLAY]", false, StructuredOutput.OutputMode.WARNING);
            foreach (string command in _history)
            {
                ExecuteCommand(command, true);
            }
            Repl.Log("[END REPLAY]", false, StructuredOutput.OutputMode.WARNING);
        }
    }
}
