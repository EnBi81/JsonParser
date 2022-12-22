namespace Parser.JsonValues;

public class StringValue : IJsonValue
{
    public string Name { get; }
    public string FullPath { get; }
    public JsonValueType Type => JsonValueType.String;
    public string Value { get; }
    
    
    public StringValue(string name, string path, string value)
    {
        Name = name;
        FullPath = path;
        Value = value;
    }
}