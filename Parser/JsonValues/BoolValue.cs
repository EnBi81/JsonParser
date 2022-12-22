namespace Parser.JsonValues;

public class BoolValue : IJsonValue
{
    public bool Value { get; }
    public string Name { get; }
    public string FullPath { get; }
    public JsonValueType Type => JsonValueType.Boolean;

    public BoolValue(string name, string path, bool value)
    {
        Name = name;
        FullPath = path;
        Value = value;
    }
}