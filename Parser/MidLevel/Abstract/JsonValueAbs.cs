using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public abstract class JsonValueAbs : IJsonValue
{
    public string Name { get; }
    public string FullPath { get; }
    public string LookUpPath { get; }
    public JsonValueType Type { get; }
    public int Depth { get; }
    public JsonMemberType JsonMemberType { get; }


    protected JsonValueAbs(string name, string fullPath, string lookUpPath, JsonValueType type, int depth, JsonMemberType memberType)
    {
        Name = name;
        FullPath = fullPath;
        LookUpPath = lookUpPath;
        Type = type;
        Depth = depth;
        JsonMemberType = memberType;
    }
}