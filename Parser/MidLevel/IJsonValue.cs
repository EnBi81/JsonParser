using System.Text.Json.Serialization;
using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

[JsonDerivedType(typeof(ArrayValue))]
[JsonDerivedType(typeof(BoolValue))]
[JsonDerivedType(typeof(NullValue))]
[JsonDerivedType(typeof(NumberValue))]
[JsonDerivedType(typeof(ObjectValue))]
[JsonDerivedType(typeof(StringValue))]
public interface IJsonValue
{
    public string? Name { get; }
    public string FullPath { get; }
    public JsonValueType Type { get; }
    public int Depth { get; }
    public JsonMemberType JsonMemberType { get; }
}