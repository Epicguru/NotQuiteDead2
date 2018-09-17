
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class CommandExec
{
    public static Dictionary<string, List<DebugCmd>> commands = new Dictionary<string, List<DebugCmd>>();

    public static void LoadCommands()
    {
        Assembly a = typeof(CommandExec).Assembly;
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
        if (commands.ContainsKey(key))
        {
            // Check for same name, same signiture commands...
            var list = commands[key];
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
            commands.Add(key, list);
            return true;
        }
    }

    [DebugCommand("Clears the command window.", GodModeOnly = false, Parameters = "FLOAT:X:The x position., FLOAT:Y:The y position")]
    public static void TestCommand()
    {

    }
}