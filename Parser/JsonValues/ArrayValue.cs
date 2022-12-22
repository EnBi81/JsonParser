namespace Parser.JsonValues;

public class ArrayValue : IJsonValue
{
    public string? Name { get; }
    public string FullPath { get; }
    public JsonValueType Type => JsonValueType.Array;
    public IEnumerable<IJsonValue> Values { get; }

    public ArrayValue(string? name, string path, IEnumerable<IJsonValue> values)
    {
        Name = name;
        FullPath = path;
        Values = values;
    }
}