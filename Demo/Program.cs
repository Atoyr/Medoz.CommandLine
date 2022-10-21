// See https://aka.ms/new-console-template for more information
using Medoz.CommandLine;
var app = Cli.NewApp();

// Application Setting
app.Version = "0.1.0";
app.Usage = "Medoz.CommandLine package demo.";
app.Authors.Add("Ryota Uchiyama");

// Set Default Action
app.Action = (_) => Console.WriteLine("Hello, World!");

// Command Action
var echoCommand = new Command("echo");
var echoCommand_valueFlag = new StringFlag("value");
echoCommand_valueFlag.Alias = new string[] {"v"};

echoCommand.Flags.Add(echoCommand_valueFlag);
echoCommand.Action = (Context ctx) => {
    var value = ctx.String("value");
    if (string.IsNullOrEmpty(value))
    {
        Console.WriteLine(value);
    }
};
// Sub Command
var echoSubCommand = new Command("revers");
var echoSubCommand_valueFlag = new StringFlag("value");
echoSubCommand_valueFlag.Alias = new string[] {"v"};

echoSubCommand.Alias = new string[] {"r"};
echoSubCommand.Action = (Context ctx) => {
    var value = ctx.String("value");
    if (string.IsNullOrEmpty(value))
    {
        string s = string.Empty;
        foreach (var c in value)
        {
            s = c.ToString() + s;
        }
        Console.WriteLine(s);
    }
};
echoCommand.SubCommands.Add(echoSubCommand);

// set Command
app.Commands.Add(echoCommand);

// Run Application
app.Run(args);
