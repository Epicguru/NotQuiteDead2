
using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class DebugCommandAttribute : Attribute
{
    public string Description { get; private set; }

    public string Parameters { get; set; } = null;
    public bool GodModeOnly { get; set; } = true;

    public DebugCommandAttribute(string description)
    {
        Description = description;
    }

    private static List<DCP> temp = new List<DCP>();
    public DCP[] GetParameters()
    {
        if (string.IsNullOrWhiteSpace(this.Parameters))
        {
            return null;
        }
        string[] parts = Parameters.Split(',');

        foreach(var part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                string error;
                var p = DCP.Create(this.Parameters, out error);
                if(p == null)
                {
                    Debug.LogError("Debug command param parse error: '{0}'".Form(error));
                }
                else
                {
                    temp.Add(p);
                }
            }
        }
        var final = temp.ToArray();
        temp.Clear();
        return final;
    }
}