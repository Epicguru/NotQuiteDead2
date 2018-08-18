using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

public class UnityContractResolver : DefaultContractResolver
{
    public static readonly UnityContractResolver Instance = new UnityContractResolver();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);


        // Ignore all properties, because they commonly have self referencing loops in Unity types such as Vectors and Rects.
        if (member.MemberType == MemberTypes.Property)
        {
            // Serialze the following property names:
            string[] excluded = new string[]
            {
                "x",
                "y"
            };

            foreach (var name in excluded)
            {
                if (member.Name == name)
                {
                    return property;
                }
            }

            if (!member.IsDefined(typeof(SerializePropertyAttribute)))
            {
                property.Ignored = true;
                return property;
            }            
        }

        return property;
    }
}