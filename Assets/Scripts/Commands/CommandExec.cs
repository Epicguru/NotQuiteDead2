
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CommandExec
{
    public static void GetCommandAttributes()
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

        Debug.Log("Found {0} classes with commands in them:".Form(found.Count()));

        foreach (var cmd in found)
        {
            var type = cmd.Type;
            var method = cmd.Method;
            var attr = cmd.Attribute;

            Debug.Log("Class: {0}, Method: {1}, Description: {2}".Form(type.FullName, method.Name, attr.Description));
        }
    }

    [DebugCommand("Clears the command window.", Parameters = "FLOAT:X:The x position, FLOAT:Y:The y position")]
    public static void TestCommand()
    {

    }
}