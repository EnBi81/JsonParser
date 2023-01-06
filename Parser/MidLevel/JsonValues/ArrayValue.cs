using System.Collections.Generic;
using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public class ArrayValue : JsonValueAbs
{
    public IEnumerable<IJsonValue> Values { get; }

    public ArrayValue(string name, string path, IEnumerable<IJsonValue> values, int depth, JsonMemberType memberType) : base(name, path, JsonValueType.Array, depth, memberType)
    {
        Values = values;
    }
}