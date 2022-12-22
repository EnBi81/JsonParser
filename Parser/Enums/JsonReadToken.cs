namespace Parser.JsonValues;

public enum JsonReadToken
{
    StartObject,
    EndObject,
    StartArray,
    EndArray,
    PropertyName,
    Number,
    String,
    Null,
    Bool,
}