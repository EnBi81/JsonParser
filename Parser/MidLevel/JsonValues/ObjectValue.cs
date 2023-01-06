using System.Collections.Generic;
using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public class ObjectValue : JsonValueAbs
{
    public IEnumerable<IJsonValue> Properties { get; }


    public ObjectValue(string? name, string path, IEnumerable<IJsonValue> properties, int depth, JsonMemberType memberType) : base(name, path, JsonValueType.Object, depth, memberType)
    {
        Properties = properties;
    }
}