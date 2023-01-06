using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public class NumberValue : JsonValueAbs
{
    public double Value { get; }
    
    public NumberValue(string name, string path, string lookUpPath, double value, int depth, JsonMemberType memberType) : 
        base(name, path, lookUpPath, JsonValueType.Number, depth, memberType)
    {
        Value = value;
    }
}