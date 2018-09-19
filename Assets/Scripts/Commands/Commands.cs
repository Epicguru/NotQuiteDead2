
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class Commands
{
    public static Dictionary<string, List<DebugCmd>> Loaded = new Dictionary<string, List<DebugCmd>>();

    public static void LoadCommands()
    {
        Loaded.Clear();

        Assembly a = typeof(Commands).Assembly;
        Debug.Log("Searching for custom commands and variables in assembly '{0}'".Form(a));

        // Partition on the type list initially.
        var def = typeof(DebugCommandAttribute);
        var found = from t in a.GetTypes().AsParallel()
            let methods = t.GetMethods()
            from m in methods
                where m.IsDefined(def, true)
                    select new { Type = t, Method = m, Attribute = m.GetCustomAttribute<DebugCommandAttribute>() };

        int count = 0;
        StringBuilder str = new StringBuilder();
        foreach (var cmd in found)
        {
            var type = cmd.Type;
            var method = cmd.Method;
            var attr = cmd.Attribute;

            if (!method.IsStatic)
            {
                Debug.LogError("Currently non-static debug commands are not supported: {0}.{1}".Form(type.FullName, method.Name));
                continue;
            }

            DebugCmd c;
            AddCmd(c = new DebugCmd(attr, method));

            str.Append("Class: {0}, Method: {1}, Info:\n{2}".Form(type.FullName, method.Name, c.GetHelp()));
            count++;
        }

        Debug.Log("Found {0} debug commands:\n{1}".Form(count, str.ToString()));
    }

    private static bool AddCmd(DebugCmd cmd)
    {
        if (cmd == null)
            return false;

        var key = cmd.Name;
        if (Loaded.ContainsKey(key))
        {
            // Check for same name, same signiture commands...
            var list = Loaded[key];
            foreach (var comm in list)
            {
                if(comm.IsVariationOf(cmd) && comm.IsSameSigniture(cmd))
                {
                    return false;
                }
            }

            // Add to list.
            list.Add(cmd);
            return true;
        }
        else
        {
            // No other commands with name, make new 'pool'.
            var list = new List<DebugCmd>();
            list.Add(cmd);
            Loaded.Add(key, list);
            return true;
        }
    }

    private static object[] ParseArgsFromInput(string input, DebugCmd cmd, out string error)
    {
        // The input string where the command part (/something) has been removed from it.
        // Exctract the inputs from the string, based on a command.

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Null or empty string!";
            return null;
        }

        const char SEP = ',';
        var parts = input.Split(SEP);
        if(parts.Length != cmd.ParameterCount)
        {
            error = "Expected {0} parameters, got {1}!".Form(cmd.ParameterCount, parts.Length);
            return null;
        }

        int i = 0;
        foreach (var param in cmd.Parameters)
        {
            string part = parts[i++];
            var type = param.Type;
            object result = null;
            switch (type)
            {
                case DCPType.STRING:
                    if(!string.IsNullOrWhiteSpace(part))
                        result = part.Trim();
                    break;
                case DCPType.FLOAT:
                    float resF;
                    bool workedF = float.TryParse(part.Trim(), out resF);
                    if (workedF)
                        result = resF;
                    break;
                case DCPType.INT:
                    int resI;
                    bool workedI = int.TryParse(part.Trim(), out resI);
                    if (workedI)
                        result = resI;
                    break;
                default:
                    result = null;
                    break;
            }

            if(result == null)
            {
                // Something went wrong!

            }
        }
    }

    private static void ExecuteCmd(DebugCmd cmd, params object[] args)
    {
        try
        {
            // Assume it is static, so invoke using a null object.
            cmd.Method.Invoke(null, args);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception while executing command '{0}', See: {1}".Form(cmd.Name, e.ToString()));
        }
    }

    [DebugCommand("Does absolutely nothing at all.", GodModeOnly = false)]
    public static void TestCommand()
    {
        Debug.Log("Whoo! Nothing!");
    }

    [DebugCommand("Does absolutely nothing at all, second edition.", GodModeOnly = false, Parameters = "STRING:thing:A random parameter.")]
    public static void TestCommand(string input)
    {
        Debug.Log("Whoo! Nothing!");
    }

    [DebugCommand("Another test.")]
    public static void CoolFing()
    {

    }
}