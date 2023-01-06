using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public class NullValue : JsonValueAbs
{
    public NullValue(string name, string fullPath, string lookUpPath, int depth, JsonMemberType memberType) :
        base(name, fullPath, lookUpPath, JsonValueType.Null, depth, memberType)
    {
    }
}