
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class Commands
{
    public static Dictionary<string, List<DebugCmd>> Loaded = new Dictionary<string, List<DebugCmd>>();
    public static List<string> PreviousCommands = new List<string>();
    private static List<object> tempParams = new List<object>();
    private static List<SortedCommandName> autocomplete = new List<SortedCommandName>();

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

            DebugCmd c = new DebugCmd(attr, method);
            string error = c.GetError();
            if(error != null)
            {
                Debug.LogError("Error with debug command {0}: {1}".Form(c.ToString(), error));
                continue;
            }
            AddCmd(c);

            str.Append("Class: {0}, Method: {1}, Info:\n{2}\n".Form(type.FullName, method.Name, c.GetHelp()));
            count++;
        }

        Debug.Log("Found {0} debug commands:\n{1}\n".Form(count, str.ToString()));
    }

    public static void Log(string output)
    {
        if(UI_CommandInput.Instance != null)
        {
            UI_CommandInput.Instance.Output.Log(output.Trim());
        }
        Debug.Log("[CMD] {0}".Form(output.Trim()));
    }

    public static List<SortedCommandName> GetAutocompleteCommand(string current)
    {
        if (string.IsNullOrWhiteSpace(current))
        {
            // Just give all commands.
            foreach (var item in Loaded.Keys)
            {
                autocomplete.Add(new SortedCommandName() { CommandName = item, Keyword = "" });
            }
            return autocomplete;
        }

        // Where current is the command being typed, with no forward slash and no arguments.
        // Get the most likely command that was meant to be typed. Detect arguments being typed, and if there are any then don't offer autocomplete.

        current = current.Trim();
        string[] split = current.Split(' ');
        if(split.Length > 1)
        {
            // Since commands are always one single word (or multiple words with underscores) then if there are more than one words, there are probably arguments.
            return null;
        }

        autocomplete.Clear();

        // TODO:
        // 1. Add all command names to the list.
        // 2. Rank based on:
        // A: Starts with current
        // B: Contains current

        foreach (var key in Loaded.Keys)
        {
            if (key.Contains(current))
                autocomplete.Add(new SortedCommandName() { CommandName = key, Keyword = current });
        }

        autocomplete.Sort();

        return autocomplete;
    }

    public static void AddCommandAsExectued(string cmd)
    {
        const int LIMIT = 100;

        cmd = cmd.Trim();

        if (PreviousCommands.Contains(cmd))
        {
            PreviousCommands.Remove(cmd);
            PreviousCommands.Insert(0, cmd);
        }
        else
        {
            PreviousCommands.Insert(0, cmd);
            if(PreviousCommands.Count > LIMIT)
            {
                PreviousCommands.RemoveAt(PreviousCommands.Count - 1);
            }
        }
    }

    public static void ClearLog()
    {
        if (UI_CommandInput.Instance != null)
        {
            UI_CommandInput.Instance.Output.ClearLog();
        }
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

    public static bool TryExecute(string input, out string error)
    {
        // Returns true to clear the command line, or false to leave as-is.

        error = null;
        input = input.Trim();

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "No command typed!";
            return false;
        }

        // Format is:
        // /cmdname x, y, z
        try
        {
            string cmd = input.Split(' ')[0].Replace("/", "");
            int length = cmd.Length + 1;
            string args = input.Substring(length);
            var cmdObject = GetCommand(cmd, args);
            if(cmdObject == null)
            {
                // Did not find command for that name or those args.
                error = "Could not find command '{0}' using args ({1})".Form(cmd, args);
                return false;
            }
            else
            {
                bool worked = ExecuteFromLine(cmdObject, input);

                if (worked)
                {
                    return true;
                }
                else
                {
                    error = "Internal error executing command: possible exception in command execution code?";
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Unknown error parsing and executing command: " + e);
            error = "Unknown internal error: " + e.Message.Trim();
            return false;
        }
    }

    private static DebugCmd GetCommand(string name, string args)
    {
        // For now, only the number of arguments is examined.
        // Therefore it is assumed that commands with the same name should have different numbers of arguments.
        // TODO fix this to allow OOP-like method overloading.
        int argCount = args.Count(x => x == ',') + 1;
        if (string.IsNullOrWhiteSpace(args))
            argCount = 0;

        if (!Loaded.ContainsKey(name))
        {
            return null;
        }
        else
        {
            var list = Loaded[name];
            foreach (var cmd in list)
            {
                if(cmd.ParameterCount == argCount)
                {
                    return cmd;
                }
            }

            Debug.LogWarning("Found commands for name '{0}', but none of them had the correct arguments (type or position)!".Form(name));
            return null;
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

        tempParams.Clear();
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
                error = "Error when parsing parameter '{0}': Expected type {1}".Form(param.Name, param.Type.ToString());
                return null;
            }
            else
            {
                tempParams.Add(result);
            }
        }

        error = null;
        return tempParams.ToArray();
    }

    private static bool ExecuteFromLine(DebugCmd cmd, string fullCommand)
    {
        // Assume both command and args are valid...

        var argsString = fullCommand.Remove(0, cmd.Name.Length + 1);
        if (string.IsNullOrWhiteSpace(argsString))
        {
            // No args, lets just do command anyway.
            return ExecuteCmd(cmd);
        }
        else
        {
            // Has args, turn input in parameters and pass them to command.

            string error;
            object[] paramz = ParseArgsFromInput(argsString.Trim(), cmd, out error);

            if(paramz == null || error != null)
            {
                Debug.LogError("Command execution [{1}] failed, argument parsing gave error! Error: '{0}'".Form(error, cmd));
                return false;
            }

            return ExecuteCmd(cmd, paramz);
        }
    }

    private static bool ExecuteCmd(DebugCmd cmd, params object[] args)
    {
        // Assume both command and args are valid...

        try
        {
            // Assume it is static, so invoke using a null object.
            cmd.Method.Invoke(null, args);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Exception while executing command '{0}', See: {1}".Form(cmd, e.ToString()));
            return false;
        }
    }
}

public class SortedCommandName : IComparable<SortedCommandName>
{
    public string CommandName;
    public string Keyword;

    public int CompareTo(SortedCommandName other)
    {
        return this.GetStartIndex() - other.GetStartIndex();
    }

    public int GetStartIndex()
    {
        return CommandName.IndexOf(Keyword);
    }
}