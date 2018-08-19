
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CommandExec
{
    private static void GetCommandAttributes()
    {
        Assembly a = typeof(CommandExec).Assembly;
        Console.WriteLine("Searching for custom commands and variables in assembly '{0}'".Form(a));

        // Partition on the type list initially.
        var found = from t in a.GetTypes().AsParallel()
            let attributes = t.GetCustomAttributes(typeof(DebugCommandAttribute), true)
            where attributes != null && attributes.Length > 0
            select new { Type = t, Methods = from x in t.GetMethods() where x.IsDefined(typeof(DebugCommandAttribute), true), Attributes = attributes.Cast<DebugCommandAttribute>() };

        foreach (var attr in found)
        {
            var type = attr.Type;
            var methods = attr.Methods;
            var attributes = attr.Attributes;

            Debug.Log("Type '{0}' has {1} attributes with {2} methods:\n".Form(type.FullName, attributes.Count(), methods.Count()));
        }
    }
}