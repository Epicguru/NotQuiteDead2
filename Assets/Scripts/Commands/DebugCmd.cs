using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

    public bool IsVariationOf(DebugCmd other)
    {
        return other != null && other.Name == this.Name;
    }

    public string GetHelp()
    {
        str.Clear();

        const string SEP = ", ";
        const string TYPE_START = " (";
        const string TYPE_END = ") - ";
        const string WHITESPACE = "  ";

        str.Append(Name);
        str.Append(WHITESPACE);
        for (int i = 0; i < ParameterCount; i++)
        {
            var param = Parameters[i];
            str.Append(param.Name);
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
            var param = Parameters[i];
            str.Append(param.Name);
            str.Append(TYPE_START);
            str.Append(param.Type.ToString().ToLower());
            str.Append(TYPE_END);
            str.Append(param.Description);
            if (i != ParameterCount - 1)
            {
                str.Append(SEP);
                str.AppendLine();
            }
        }

        return str.ToString().TrimEnd();
    }
}

