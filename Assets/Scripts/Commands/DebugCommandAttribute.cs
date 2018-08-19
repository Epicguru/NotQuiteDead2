
using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class DebugCommandAttribute : Attribute
{
    public string Description { get; private set; }

    public DebugCommandAttribute(string description)
    {
        Description = description;
    }
}