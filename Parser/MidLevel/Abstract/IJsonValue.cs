﻿using Parser.Analyzer.Enums;

namespace Parser.JsonValues;

public interface IJsonValue
{
    public string? Name { get; }
    public string FullPath { get; }
    public JsonValueType Type { get; }
    public int Depth { get; }
    public JsonMemberType JsonMemberType { get; }
}