using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public class NullValue : JsonValueAbs
{
    public NullValue(string name, string fullPath, int depth, JsonMemberType memberType) : base(name, fullPath, JsonValueType.Null, depth, memberType)
    {
    }
}