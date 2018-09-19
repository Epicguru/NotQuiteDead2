using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DebugCmd
{
    private static StringBuilder str = new StringBuilder();

    public string Name { get; private set; }
    public string Description
    {
        get
        {
            return Attribute.Description;
        }
    }
    public DebugCommandAttribute Attribute { get; private set; }
    public int ParameterCount
    {
        get
        {
            return Parameters == null ? 0 : Parameters.Length;
        }
    }
    public DCP[] Parameters { get; private set; }
    public MethodInfo Method { get; private set; }

    public DebugCmd(DebugCommandAttribute attr, MethodInfo method)
    {
        // It's late, lets just assume that attr and method are not null and are both valid.
        Attribute = attr;

        // Name.
        Name = method.Name.Trim().ToLower();
        // Description.
        Attribute.Description.Trim();
        // Method.
        this.Method = method;
        // Parameters.
        if (string.IsNullOrWhiteSpace(Attribute.Parameters)) Attribute.Parameters = null;
        this.Parameters = Attribute.GetParameters();
    }

    public bool IsVariationOf(DebugCmd other)
    {
        return other != null && other.Name == this.Name;
    }

    public bool IsSameSigniture(DebugCmd other)
    {
        if (other == null)
            return false;

        if(other.ParameterCount != this.ParameterCount)
        {
            return false;
        }

        for (int i = 0; i < ParameterCount; i++)
        {
            var tp = this.Parameters[i];
            var op = other.Parameters[i];

            if (!tp.SameAs(op))
                return false;
        }

        return true;
    }

    public string GetHelp()
    {
        str.Clear();

        const string INDENT = "  > ";
        const string SEP = ", ";
        const string TYPE_START = " (";
        const string TYPE_END = ") - ";
        const string WHITESPACE = "  ";

        str.Append(Name);
        str.Append(WHITESPACE);
        for (int i = 0; i < ParameterCount; i++)
        {
            var param = Parameters[i];
            str.Append(param.Name.Trim().ToLower());
            if(i != ParameterCount - 1)
            {
                str.Append(SEP);
            }
        }
        str.AppendLine();
        str.Append(Description);
        str.AppendLine();
        for (int i = 0; i < ParameterCount; i++)
        {
            str.Append(INDENT);
            var param = Parameters[i];
            str.Append(param.Name.ToLower().Trim());
            str.Append(TYPE_START);
            str.Append(param.Type.ToString().ToLower());
            str.Append(TYPE_END);
            string desc = param.Description.Trim();
            if (desc.EndsWith("."))
                desc = desc.Remove(desc.Length - 1, 1);
            str.Append(desc);
            if (i != ParameterCount - 1)
            {
                str.Append(SEP);
                str.AppendLine();
            }
        }

        return str.ToString().TrimEnd();
    }
}

