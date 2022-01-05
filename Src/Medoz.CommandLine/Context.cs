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

    public string String(string key)
    {
        if (FlagValues.ContainsKey(key)) return (string)FlagValues[key];
        return GlobalString(key);
    }

    public string GlobalString(string key)
    {
        if (GlobalFlagValues.ContainsKey(key)) return (string)GlobalFlagValues[key];
        return string.Empty; // TODO Throw Exception
    }

    public int Int(string key)
    {
        if (FlagValues.ContainsKey(key)) return (int)FlagValues[key];
        return GlobalInt(key);
    }

    public int GlobalInt(string key)
    {
        if (GlobalFlagValues.ContainsKey(key)) return (int)GlobalFlagValues[key];
        return 0; // TODO Throw Exceptinon
    }

    public bool Bool(string key)
    {
        if (FlagValues.ContainsKey(key)) return (bool)FlagValues[key];
        return GlobalBool(key);
    }

    public bool GlobalBool(string key)
    {
        if (GlobalFlagValues.ContainsKey(key)) return (bool)GlobalFlagValues[key];
        return false; // TODO Throw Exception
    }
}
