using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using Parser.Analyzer.Enums;
using Parser.JsonValues;

namespace Parser;

public class JsonAnalyzer
{
    private readonly JsonDocumentIterator _documentIterator;

    public IEnumerable<ArrayValue> Iterators => _iterators;
    private List<ArrayValue> _iterators = new ();

    public JsonAnalyzer(Stream stream)
    {
        _documentIterator = new JsonDocumentIterator(stream);
    }

    public IJsonValue Analyze()
    {
        IJsonValue rootObj = _documentIterator.Parse();
        return LoadAnalyzeData(rootObj);
    }

    /// <summary>
    /// Loads all the static objects into the memory + the first 100 values of the iterators
    /// </summary>
    /// <param name="jsonValue"></param>
    /// <returns></returns>
    private IJsonValue LoadAnalyzeData(IJsonValue jsonValue)
    {
        if (jsonValue.JsonMemberType is JsonMemberType.Iterator && jsonValue is ArrayValue arr)
        {
            // load the first 100 objects into the memory
            ICollection<IJsonValue> memoryObjects = RetrieveMemoryIteratorObjects(arr, 100);
            var newArray = new ArrayValue(arr.Name, arr.FullPath, arr.LookUpPath, memoryObjects, arr.Depth, arr.JsonMemberType);
            _iterators.Add(newArray);
            return newArray;
        }

        // an instance or a multivalued object should not be here, they are 
        // either going through the 'LoadObjectToMemory' or the 'RunThroughJson' method
        if (jsonValue.JsonMemberType is not JsonMemberType.Static)
            throw new Exception("Unexpected json member type: " + jsonValue.JsonMemberType);
        
        // now the object is static
        if (jsonValue is not ObjectValue obj)
            return jsonValue;

        List<IJsonValue> memoryValues = new();
        foreach (IJsonValue prop in obj.Properties)
        {
            IJsonValue memoryJsonValue = LoadAnalyzeData(prop);
            memoryValues.Add(memoryJsonValue);
        }

        return new ObjectValue(obj.Name, obj.FullPath, obj.LookUpPath, memoryValues, obj.Depth, obj.JsonMemberType);
    }

    /// <summary>
    /// Loads the first x elements of a list to the memory, where x = <see cref="objectsToSave"/>. Discards the rest of the array elements
    /// </summary>
    /// <param name="arrayValue"></param>
    /// <param name="objectsToSave"></param>
    /// <returns></returns>
    private ICollection<IJsonValue> RetrieveMemoryIteratorObjects(ArrayValue arrayValue, int objectsToSave)
    {
        int count = 0;
        List<IJsonValue> values = new List<IJsonValue>();
        
        foreach (IJsonValue value in arrayValue.Values)
        {
            count++;
            // load into the list
            if (count <= objectsToSave)
            {
                IJsonValue memoryJson = LoadObjectToMemory(value);
                values.Add(memoryJson);
            }
            // else skip the object
            else
            {
                RunThroughJson(value);
            }
        }

        return values;
    }

    /// <summary>
    /// Loads an object into the memory and returns the loaded object
    /// </summary>
    /// <param name="jsonValue"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private IJsonValue LoadObjectToMemory(IJsonValue jsonValue)
    {
        // check if jsonValue is either a full object or an array
        IEnumerable<IJsonValue>? values = jsonValue switch
        {
            ArrayValue arrayValue => arrayValue.Values,
            ObjectValue objectValue => objectValue.Properties,
            _ => null
        };

        // if the jsonValue doesnt contain an IEnumerable, it means everything is loaded into the memory,
        // so just return the object itself
        if (values is null)
            return jsonValue;

        List<IJsonValue> memoryValues = new ();
        foreach (IJsonValue value in values)
        {
            IJsonValue memoryValue = LoadObjectToMemory(value);
            memoryValues.Add(memoryValue);
        }

        if (jsonValue is ArrayValue arr)
            return new ArrayValue(arr.Name, arr.FullPath, arr.LookUpPath, memoryValues, arr.Depth, arr.JsonMemberType);

        if (jsonValue is ObjectValue obj)
            return new ObjectValue(obj.Name, obj.FullPath, obj.LookUpPath, memoryValues, obj.Depth, obj.JsonMemberType);

        throw new Exception("Unexpected json value: " + jsonValue);
    }

    /// <summary>
    /// Runs through a json object but doesn't save it to the memory
    /// </summary>
    private void RunThroughJson(IJsonValue jsonValue)
    {
        // check if jsonValue is either a full object or an array
        IEnumerable<IJsonValue>? values = jsonValue switch
        {
            ArrayValue arr => arr.Values,
            ObjectValue obj => obj.Properties,
            _ => null
        };

        // if not, then move forward
        if (values is null)
            return;

        // if its an array or an object, then loop through the elements (so that the stream reads it
        foreach (IJsonValue v in values)
            RunThroughJson(v);
    }
}