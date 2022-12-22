using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Parser.JsonValues;

namespace Parser;

public class JsonDocumentIterator
{
    private readonly JsonTextReader reader;
    
    public JsonDocumentIterator(Stream stream)
    {
        reader = new JsonTextReader(stream);
    }

    public IJsonValue Parse()
    {
        return ReadValue("", "");
    }

    private static string JoinPath(string path, string name)
    {
        if (string.IsNullOrEmpty(path))
            return name;
        return path + "." + name;
    }

    /// <summary>
    /// First character if the object must be read '{'
    /// </summary>
    /// <param name="absolutePath"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private IEnumerable<IJsonValue> ParseObject(string absolutePath)
    {
        while (reader.ReadNextToken())
        {
            if (reader.CurrentToken == JsonReadToken.EndObject)
            {
                yield break;
            }

            if (reader.CurrentToken is JsonReadToken.PropertyName)
            {
                string name = (string)reader.CurrentValue!;
                string path = JoinPath(absolutePath, name);
                yield return ReadValue(name, path);
            }
        }
    }

    /// <summary>
    /// First character of the array must be read '['
    /// </summary>
    /// <param name="absolutePath"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private IEnumerable<IJsonValue> ParseArray(string absolutePath)
    {
        int count = 0;
        while (true)
        {
            IJsonValue value;
            try
            {
                string path = JoinPath(absolutePath, count + "");
                value = ReadValue(count + "", path);
                count++;
            }
            catch (Exception)
            {
                yield break;
            }

            yield return value;
        }
    }

    /// <summary>
    /// Reads a value from the stream and returns it.
    /// </summary>
    /// <returns></returns>
    private IJsonValue ReadValue(string propertyName, string absolutePath)
    {
        if(!reader.ReadNextToken())
            throw new Exception("Unexpected end of stream");
        
        var currentToken = reader.CurrentToken;
        
        if (currentToken is JsonReadToken.Number)
        {
            var value = (double)reader.CurrentValue!;
            return new NumberValue(propertyName, absolutePath, value);
        }
        if (currentToken is JsonReadToken.String)
        {
            var value = (string)reader.CurrentValue!;
            return new StringValue(propertyName, absolutePath, value);
        }
        if (currentToken is JsonReadToken.Bool)
        {
            var value = (bool)reader.CurrentValue!;
            return new BoolValue(propertyName, absolutePath, value);
        }
        if (currentToken is JsonReadToken.Null)
        {
            return new NullValue(propertyName, absolutePath);
        }
        if (currentToken is JsonReadToken.StartArray)
        {
            IEnumerable<IJsonValue> enumerable = ParseArray(absolutePath);
            return new ArrayValue(propertyName, absolutePath, enumerable);
        }
        if (currentToken is JsonReadToken.StartObject)
        {
            IEnumerable<IJsonValue> enumerable = ParseObject(absolutePath);
            return new ObjectValue(propertyName, absolutePath, enumerable);
        }
        
        throw new Exception("Unexpected token: " + currentToken);
    }
}