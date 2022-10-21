namespace Medoz.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class Cli
{
    public static Cli NewApp() => new();

    public string Name { set; get; } = Path.GetFileNameWithoutExtension(Environment.ProcessPath) ?? string.Empty;

    public string Usage { set; get; }

    public string ArgsUsage { set; get; }

    public string Version { set; get; }

    public string Description { set; get; }

    public IList<string> Authors { set; get; }

    public string CopyRight { set; get; }

    public string ProcessName { get; } = Path.GetFileNameWithoutExtension(Environment.ProcessPath) ?? string.Empty;

    public IList<Flag> Flags { set; get; }

    public IList<Command> Commands { set; get; }

    public Action<Context> Action { set; get; }

    public bool HideHelpCommand { set; get; } = false;
    public bool HideHelpFlag { set; get; } = false;

#pragma warning disable CS8618
    public Cli()
    {
        Authors = new List<string>();
        Flags = new List<Flag>();
        Commands = new List<Command>();
    }
#pragma warning restore CS8618

    public void Run(string[] args)
    {
        applyHelpAction();
        var ctx = new Context();

        // set initial value
        foreach (var flag in Flags)
        {
            ctx.GlobalFlagValues.Add(flag.Name, flag.Value);
        }

        Action<Context> action = Action == null ? writeHelpText : Action;

        // setting for Contexts Flag and Args
        Command command = null;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            // Option
            if (arg.Substring(0, 1) == "-")
            {
                string optionName = arg.Substring(0, 2) == "--" ? arg.Substring(2) : arg.Substring(1);

                if (command is null)
                {
                    // Global Options
                    if (!Flags.Any(x => x.Names().Any(x => x == optionName))) continue;
                    Flag flag = Flags.First(x => x.Names().Any(x => x == optionName));
                    if (flag is BoolFlag)
                    {
                        ctx.GlobalFlagValues[flag.Name] = true;
                    }
                    else
                    {
                        if (i + 1 < args.Length) ctx.GlobalFlagValues[flag.Name] = args[++i];
                    }
                }
                else
                {
                    // Command Options
                    Flag flag;
                    if (command.Flags.Any(x => x.Names().Any(x => x == optionName)))
                    {
                        flag = command.Flags.First(x => x.Names().Any(x => x == optionName));
                    }
                    else if (Flags.Any(x => x.Names().Any(x => x == optionName)))
                    {
                        flag = Flags.First(x => x.Names().Any(x => x == optionName));
                    }
                    else
                    {
                        continue;
                    }

                    if (flag is BoolFlag)
                    {
                        ctx.FlagValues[flag.Name] = true;
                    }
                    else
                    {
                        if (i + 1 < args.Length) ctx.FlagValues[flag.Name] = args[++i];
                    }
                }
            }
            else
            {
                // Command or args
                if (command is null)
                {
                    if (Commands.Any(x => x.Names().Any(x => x == arg)))
                    {
                        command = Commands.First(x => x.Names().Any(x => x == arg));
                    }
                    else
                    {
                        ctx.Args = args.Skip(i).ToArray();
                        break;
                    }
                }
                else
                {
                    if (command.SubCommands.Any(x => x.Names().Any(x => x == arg)))
                    {
                        command = command.SubCommands.First(x => x.Names().Any(x => x == arg));
                    }
                    else
                    {
                        ctx.Args = args.Skip(i).ToArray();
                        break;
                    }

                }
            }
        }

        if (command is not null)
        {
            action = command.Action;
        }

        action.Invoke(ctx);
    }

    private void applyHelpAction()
    {
        if (!HideHelpCommand)
        {
            var helpCommand = new Command("help");
            helpCommand.Alias = new string[] { "h" };
            helpCommand.Usage = "Shows a list of commands or help for one command";
            helpCommand.Action = writeHelpText;
            Commands.Add(helpCommand);
        }

        if (!HideHelpFlag)
        {
            var helpFlag = new BoolFlag("help");
            helpFlag.Alias = new string[] { "h" };
            helpFlag.Usage = "Shows a list of commands or help for one command";
            helpFlag.SetDefaultValue(false);
            Flags.Add(helpFlag);
        }
    }

    private void writeHelpText(Context ctx)
    {
        Console.Write(appHelpText());
    }

    private string appHelpText()
    {
        StringBuilder sb = new StringBuilder();
        // NAME
        sb.Append($"Name: \n");
        sb.Append($"\t{Name} {(string.IsNullOrWhiteSpace(Usage) ? "" : "- " + Usage)}");
        sb.Append($"\n");

        // USAGE
        sb.Append($"Usage: \n");
        sb.Append($"\t{ProcessName}");
        if (Flags.Count() > 0)
        {
            sb.Append(" [global options]");
        }
        if (Commands.Count() > 0)
        {
            sb.Append(" command [command options]");
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

        // VERSION
        if (!string.IsNullOrWhiteSpace(Version))
        {
            sb.Append($"Version: \n");
            sb.Append($"\t{Version}");
            sb.Append($"\n");
        }

        // DESCRIPTION
        if (!string.IsNullOrWhiteSpace(Description))
        {
            sb.Append($"Description: \n");
            sb.Append($"\t{Description}");
            sb.Append($"\n");
        }

        // AUTHOR(S)
        if (Authors.Count() > 0)
        {
            if (Authors.Count() > 1)
            {
                sb.Append($"Authors: \n").Append("\t");
            }
            else
            {
                sb.Append($"Author: \n").Append("\t");
            }
            sb.Append(String.Join(", ", Authors));
            sb.Append($"\n");
        }

        // COMMANDS
        if (Commands.Count() > 0)
        {
            sb.Append($"Commands: \n");

            int maxLength = Commands.Select(x => String.Join(", ", x.Names()).Length).Max();
            foreach (var command in Commands)
            {
                string n = String.Join(", ", command.Names());
                sb.Append("\t").Append(n).Append(new string(' ', maxLength - n.Length + 1)).Append($" {command.Usage}");
                sb.Append($"\n");
            }
        }

        // GLOBAL OPTIONS
        if (Flags.Count() > 0)
        {

            sb.Append($"Global Options: \n");

            int maxLength = Flags.Select(x => String.Join(", ", x.NamesAppendHyphen()).Length).Max();
            foreach (var flag in Flags)
            {
                string n = String.Join(", ", flag.NamesAppendHyphen());
                sb.Append("\t").Append(n).Append(new string(' ', maxLength - n.Length + 1)).Append($" {flag.Usage}");
                if (flag.Value is not null)
                {
                    sb.Append($" (default: \"{flag.Value}\")");

                }
                sb.Append($"\n");
                // TODO Value
            }
        }

        // COPYLIGHT
        if (!string.IsNullOrWhiteSpace(CopyRight))
        {
            sb.Append($"Copyright: \n");
            sb.Append($"\t{CopyRight}");
            sb.Append($"\n");
        }

        return sb.ToString();
    }
}