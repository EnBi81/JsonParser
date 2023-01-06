using System.Text;
using Parser.Analyzer.Enums;
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
        return ReadValue( propertyName: "", absolutePath: "$", lookUpPath: "$", depth: 0, parentMemberType: JsonMemberType.Static);
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
    /// <param name="depth">the depth of this object</param>
    /// <returns></returns>
    private IEnumerable<IJsonValue> ParseObject(string absolutePath, string lookUpPath, int depth, JsonMemberType currentMemberType)
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
                string pathName = "['" + name + "']";
                string newAbsolutePath = absolutePath + pathName;
                string newLookUpPath = lookUpPath + pathName;
                
                yield return ReadValue(name, newAbsolutePath, newLookUpPath, depth, currentMemberType);
            }
        }
    }

    /// <summary>
    /// First character of the array must be read '['
    /// </summary>
    /// <param name="absolutePath"></param>
    /// <param name="depth">the depth of this object</param>
    /// <returns></returns>
    private IEnumerable<IJsonValue> ParseArray(string absolutePath, string lookUpPath, int depth, JsonMemberType currentMemberType)
    {
        string newLookUpPath = lookUpPath + "[" + "_" + "]";
        int count = 0;
        while (true)
        {
            IJsonValue value;
            try
            {
                string name = count + "";
                string newAbsolutePath = absolutePath + "[" + name + "]";
                value = ReadValue(name, newAbsolutePath, newLookUpPath, depth, currentMemberType);
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
    /// <param name="depth">the depth of this object</param>
    /// <returns></returns>
    private IJsonValue ReadValue(string propertyName, string absolutePath, string lookUpPath, int depth, JsonMemberType parentMemberType)
    {
        if(!reader.ReadNextToken())
            throw new Exception("Unexpected end of stream");
        
        var currentToken = reader.CurrentToken;
        var newDepth = depth + 1;

        JsonMemberType newMemberType = parentMemberType switch
        {
            JsonMemberType.Static => JsonMemberType.Static,
            JsonMemberType.Iterator => JsonMemberType.Instance,
            JsonMemberType.Instance => JsonMemberType.Instance,
            JsonMemberType.MultiValuedInstance => JsonMemberType.MultiValuedInstance,
            _ => throw new NotImplementedException("Member type " + parentMemberType + " is not implemented")
        };
        
        if (currentToken is JsonReadToken.Number)
        {
            var value = (double)reader.CurrentValue!;
            return new NumberValue(propertyName, absolutePath, lookUpPath, value, newDepth, newMemberType);
        }
        if (currentToken is JsonReadToken.String)
        {
            var value = (string)reader.CurrentValue!;
            return new StringValue(propertyName, absolutePath, lookUpPath, value, newDepth, newMemberType);
        }
        if (currentToken is JsonReadToken.Bool)
        {
            var value = (bool)reader.CurrentValue!;
            return new BoolValue(propertyName, absolutePath, lookUpPath, value, newDepth, newMemberType);
        }
        if (currentToken is JsonReadToken.Null)
        {
            return new NullValue(propertyName, absolutePath, lookUpPath, newDepth, newMemberType);
        }
        if (currentToken is JsonReadToken.StartArray)
        {
            JsonMemberType arrayMemberType = parentMemberType switch
            {
                JsonMemberType.Static => JsonMemberType.Iterator,
                JsonMemberType.Iterator => JsonMemberType.MultiValuedInstance,
                JsonMemberType.Instance => JsonMemberType.MultiValuedInstance,
                JsonMemberType.MultiValuedInstance => JsonMemberType.MultiValuedInstance,
                _ => throw new NotImplementedException("Member type " + parentMemberType + " is not implemented")
            };
            
            IEnumerable<IJsonValue> enumerable = ParseArray(absolutePath, lookUpPath, newDepth, arrayMemberType);
            return new ArrayValue(propertyName, absolutePath, lookUpPath, enumerable, newDepth, arrayMemberType);
        }
        if (currentToken is JsonReadToken.StartObject)
        {
            IEnumerable<IJsonValue> enumerable = ParseObject(absolutePath, lookUpPath, newDepth, newMemberType);
            return new ObjectValue(propertyName, absolutePath, lookUpPath, enumerable, newDepth, newMemberType);
        }
        
        throw new Exception("Unexpected token: " + currentToken);
    }
}