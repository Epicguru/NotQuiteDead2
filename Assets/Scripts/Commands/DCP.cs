using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DCP
{
    public DCPType Type { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private DCP(DCPType type, string name, string desc)
    {
        this.Type = type;
        this.Name = name;
        this.Description = desc;
    }

    public static DCP Create(string paramChunk, out string error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(paramChunk))
        {
            error = "Input is null.";
            return null;
        }

        int sepCount = paramChunk.Count<char>(x => x == ':');
        if (sepCount != 2)
        {
            error = "Invalid param format: Must be in the format 'TYPE:name:description' ({0}, {1})".Form(paramChunk, sepCount);
            return null;
        }

        string[] parts = paramChunk.Trim().Split(':');
        string type = parts[0].Trim();
        string name = parts[1].Trim();
        string desc = parts[2].Trim();

        if (string.IsNullOrWhiteSpace(type))
        {
            error = "Param type was left blank, invalid!";
            return null;
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Param name was left blank, invalid!";
            return null;
        }
        if (string.IsNullOrWhiteSpace(desc))
        {
            error = "Param description was left blank, invalid!";
            return null;
        }

        DCPType parsedType;
        bool parsed = Enum.TryParse<DCPType>(type, true, out parsedType);

        if (!parsed)
        {
            error = "'{0}' is an invalid parameter type. See the DCPType enum to get a list of valid type names.".Form(type);
            return null;
        }

        return new DCP(parsedType, name, desc);
    }

    public bool SameAs(DCP other)
    {
        return other != null && other.Type == this.Type;
    }

    public override string ToString()
    {
        return "{0} - {1}".Form(Name.ToLower(), Description.ToLower());
    }
}

public enum DCPType : byte
{
    STRING,
    INT,
    FLOAT
}

