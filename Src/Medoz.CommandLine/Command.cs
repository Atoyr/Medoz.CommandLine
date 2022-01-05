namespace Medoz.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class Command
{
    public string Name { set; get; }
    public string[] Alias { set; get; }
    public string Usage { set; get; }
    public string UsageText { set; get; }
    public string ArgsUsage { set; get; }
    public Action<Context> Action { set; get; }
    public IList<Flag> Flags { set; get; }
    public Command Parent { set; get; }
    public List<Command> SubCommands { get; }

    public string[] Names()
    {
        var names = new string[Alias.Length + 1];
        names[0] = Name;
        Array.Copy(Alias, 0, names, 1, Alias.Length);
        return names;
    }

    private Command() { }
    public Command(string name)
    {
        Name = name;
        Alias = new string[0];
        Action = (_) => Console.WriteLine(CommandHelpText());
        Flags = new List<Flag>();
        SubCommands = new List<Command>();
    }

    public void AddSubCommand(Command subCommand)
    {
        int index = SubCommands.FindIndex(x => x.Name == subCommand.Name);
        if (index < 0)
        {
            SubCommands.Add(subCommand);
        }
        else
        {
            SubCommands[index] = subCommand;
        }
    }

    private string CommandHelpText()
    {
        StringBuilder sb = new StringBuilder();
        // NAME
        sb.Append($"NAME: \n");
        sb.Append($"\t{Name} {(string.IsNullOrWhiteSpace(Usage) ? "" : "- " + Usage)}");
        sb.Append($"\n");

        // USAGE
        sb.Append($"USAGE: \n");
        sb.Append($"\t{Name}");
        if (Flags.Count() > 0)
        {
            sb.Append(" [command options]");
        }
        if (SubCommands.Count() > 0)
        {
            sb.Append(" subcommand [subcommand options]");
        }
        if (string.IsNullOrEmpty(ArgsUsage))
        {
            sb.Append(" [arguments...]");
        }
        else
        {
            sb.Append(" ").Append(ArgsUsage);
        }
        sb.Append($"\n");

        // SUBCOMMANDS
        if (SubCommands.Count() > 0)
        {
            sb.Append($"SUBCOMMANDS: \n");

            int maxLength = SubCommands.Select(x => String.Join(", ", x.Names()).Length).Max();
            foreach (var command in SubCommands)
            {
                string n = String.Join(", ", command.Names());
                sb.Append("\t").Append(n).Append(new string(' ', maxLength - n.Length + 1)).Append($" {command.Usage}");
                sb.Append($"\n");
            }
        }

        // OPTIONS
        if (Flags.Count() > 0)
        {

            sb.Append($"OPTIONS: \n");

            int maxLength = Flags.Select(x => String.Join(", ", x.NamesAppendHyphen()).Length).Max();
            foreach (var flag in Flags)
            {
                string n = String.Join(", ", flag.NamesAppendHyphen());
                sb.Append("\t").Append(n).Append(new string(' ', maxLength - n.Length + 1)).Append($" {flag.Usage}");
                sb.Append($"\n");
            }
        }
        return sb.ToString();
    }
}
