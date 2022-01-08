namespace Medoz.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class Context
{
    public IDictionary<string, object> FlagValues { set; get; }
    public IDictionary<string, object> GlobalFlagValues { set; get; }
    public string[] Args { set; get; }

#pragma warning disable CS8618
    public Context()
    {
        FlagValues = new Dictionary<string, object>();
        GlobalFlagValues = new Dictionary<string, object>();
    }
#pragma warning restore CS8618

    public string String(string key) => FlagValues.ContainsKey(key) && FlagValues[key] is string s ? s : GlobalString(key);
    public string GlobalString(string key) => GlobalFlagValues.ContainsKey(key) && GlobalFlagValues[key] is string s ? s : string.Empty; // TODO Throw Exception

    public int Int(string key) => FlagValues.ContainsKey(key) && FlagValues[key] is int i ? i : GlobalInt(key);
    public int GlobalInt(string key) => GlobalFlagValues.ContainsKey(key) && GlobalFlagValues[key] is int i ? i : 0;

    public bool Bool(string key) => FlagValues.ContainsKey(key) && FlagValues[key] is bool b ? b : GlobalBool(key);
    public bool GlobalBool(string key) => GlobalFlagValues.ContainsKey(key) && GlobalFlagValues[key] is bool b ? b : false;
}
