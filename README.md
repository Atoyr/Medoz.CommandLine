# Medoz.CommandLine
[![Nuget](https://img.shields.io/nuget/v/Medoz.CommandLine)](https://www.nuget.org/packages/Medoz.CommandLine/)
[![GitHub](https://img.shields.io/github/license/Atoyr/Medoz.CommandLine)](https://github.com/Atoyr/Medoz.CommandLine/blob/main/LICENSE)
## .NET Commandline tool
.NET command line application tool.

## prerequisites
- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

## Getting Start
To Install
```
dotnet add package Medoz.CommandLine --version 0.2.0
```

Add the following to `Program.cs`
``` cs
using Medoz.CommandLine;
var app = Cli.NewApp();

app.Run(args)
```

Set root action
``` cs
app.Action = (Context ctx) => {
    // Write your application
}
```

Set flag `-v --value` for application
``` cs
var flag = new StringFlag("value");
flag.Alias = new string[] {"v"};
app.Flags.Add(flag);
```

Use flag `-v --value` for action
``` cs
app.Action = (Context ctx) => {
    var value = ctx.String("value");
}
```

Set command
``` cs
var echoCommand = new Command("echo");
echoCommand.Action = (Context ctx) => {
    foreach(var s in ctx.Args)
    {
        Console.WriteLine(s);
    }
};
app.Commands.Add(echoCommand);
```

## Version
### 0.2.0
- fix: Help message not working
- fix: type error when getting value from context
### 0.1.0
- create project.

## License
This project is licensed under the terms of the [MIT license](LICENSE).

## Respect
this project respect [urfave/cli](https://github.com/urfave/cli).
