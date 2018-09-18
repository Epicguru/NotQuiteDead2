
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