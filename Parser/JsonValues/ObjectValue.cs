namespace Parser.JsonValues;

public class ObjectValue : IJsonValue
{
    public string? Name { get; }
    public string FullPath { get; }
    public JsonValueType Type => JsonValueType.Object;

    public IEnumerable<IJsonValue> Properties { get; }


    public ObjectValue(string? name, string path, IEnumerable<IJsonValue> properties)
    {
        Name = name;
        FullPath = path;
        Properties = properties;
    }
}