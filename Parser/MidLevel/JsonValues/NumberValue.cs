using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public class NumberValue : JsonValueAbs
{
    public double Value { get; }
    
    public NumberValue(string name, string path, double value, int depth, JsonMemberType memberType) : base(name, path, JsonValueType.Number, depth, memberType)
    {
        Value = value;
    }
}