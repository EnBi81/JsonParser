namespace Parser.JsonValues;

public class NullValue : IJsonValue
{
    public string Name { get; }
    public string FullPath { get; }
    public JsonValueType Type => JsonValueType.Null;

    public NullValue(string name, string fullPath)
    {
        Name = name;
        FullPath = fullPath;
    }
}