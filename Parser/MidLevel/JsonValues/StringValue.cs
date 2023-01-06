using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public class StringValue : JsonValueAbs
{
    public string Value { get; }
    
    
    public StringValue(string name, string path, string lookUpPath, string value, int depth, JsonMemberType memberType) :
        base(name, path,  lookUpPath, JsonValueType.String, depth, memberType)
    {
        Value = value;
    }
}