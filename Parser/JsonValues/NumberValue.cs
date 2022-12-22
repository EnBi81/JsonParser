namespace Parser.JsonValues;

public class NumberValue : IJsonValue
{
    public string Name { get; }
    public string FullPath { get; }
    public JsonValueType Type => JsonValueType.Number;
    public double Value { get; }
    
    public NumberValue(string name, string path, double value)
    {
        Value = value;
        Name = name;
        FullPath = path;
    }
}