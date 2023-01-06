using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public class BoolValue : JsonValueAbs
{
    public bool Value { get; }

    public BoolValue(string name, string path, bool value, int depth, JsonMemberType memberType) : base(name, path, JsonValueType.Boolean, depth, memberType)
    {
        Value = value;
    }
}