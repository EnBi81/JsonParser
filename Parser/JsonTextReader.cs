using System;
using System.IO;
using System.Text;
using Parser.JsonValues;

namespace Parser;

/// <summary>
/// Reads a json token by token
/// </summary>
internal class JsonTextReader
{
    private readonly Stream stream;
    private int currentByte;
    private bool shouldReadNextByte = true;
    
    
    /// <summary>
    /// Current json token
    /// </summary>
    public JsonReadToken CurrentToken { get; private set; }
    /// <summary>
    /// Current value of the token
    /// </summary>
    public object? CurrentValue { get; private set; }
    /// <summary>
    /// Depth of the current token
    /// </summary>
    public int Depth
    {
        get
        {
            if (CurrentToken is JsonReadToken.StartObject or JsonReadToken.StartArray)
                return _depth - 1;
            return _depth;
        }
    }

    private int _depth;


    public JsonTextReader(Stream stream)
    {
        this.stream = stream;
    }
    
    private bool ReadNextByte()
    {
        currentByte = stream.ReadByte();
        return currentByte != -1;
    }
    
    /// <summary>
    /// Reads the next token
    /// </summary>
    /// <returns></returns>
    public bool ReadNextToken()
    {
        int nextByte;
        
        while (!shouldReadNextByte || ReadNextByte())
        {
            shouldReadNextByte = true;
            
            nextByte = currentByte;
            CurrentValue = null;
            
            // skip the whitespace and unnecessary characters
            if(nextByte is ' ' or '\t' or '\r' or '\n' or ':' or ',')
                continue;

            // object
            if (nextByte == '{')
            {
                _depth++;
                CurrentToken = JsonReadToken.StartObject;
                return true;
            }

            if (nextByte == '}')
            {
                _depth--;
                CurrentToken = JsonReadToken.EndObject;
                return true;
            }
            
            // array
            if (nextByte == '[')
            {
                _depth++;
                CurrentToken = JsonReadToken.StartArray;
                return true;
            }
            
            if (nextByte == ']')
            {
                _depth--;
                CurrentToken = JsonReadToken.EndArray;
                return true;
            }
            
            // string can be a property name if the previous token was not a property name
            if (nextByte == '"' && CurrentToken != JsonReadToken.PropertyName)
            {
                CurrentToken = JsonReadToken.PropertyName;
                CurrentValue = FinishReadingString('"');
                return true;
            }
            
            // Read the value
            
            // string value
            if (nextByte is '"' or '\'')
            {
                CurrentToken = JsonReadToken.String;
                CurrentValue = FinishReadingString((char)nextByte);
                return true;
            }
            
            // number value
            if (nextByte is >= '0' and <= '9' or '-' or '.')
            {
                CurrentToken = JsonReadToken.Number;
                CurrentValue = FinishReadingNumber();
                shouldReadNextByte = false;
                return true;
            }
            
            // boolean value
            if (nextByte is 't' or 'f')
            {
                CurrentToken = JsonReadToken.Bool;
                CurrentValue = FinishReadingBoolean();
                return true;
            }
            
            // null value
            if (nextByte == 'n')
            {
                CurrentToken = JsonReadToken.Null;
                CurrentValue = FinishReadingNull();
                return true;
            }


            CurrentToken = JsonReadToken.Null;
            return true;
        }

        return false;
    }

    private object? FinishReadingNull()
    {
        if (stream.ReadByte() != 'u' ||
            stream.ReadByte() != 'l' ||
            stream.ReadByte() != 'l')
            throw new Exception("Invalid null value");

        return null;
    }

    private bool FinishReadingBoolean()
    {
        var isTrue = currentByte is 't';
        if (isTrue)
        {
            if (stream.ReadByte() != 'r' ||
                stream.ReadByte() != 'u' ||
                stream.ReadByte() != 'e')
                throw new Exception("Invalid boolean value");
        }
        else
        {
            if (stream.ReadByte() != 'a' ||
                stream.ReadByte() != 'l' ||
                stream.ReadByte() != 's' ||
                stream.ReadByte() != 'e')
                throw new Exception("Invalid boolean value");
        }

        return isTrue;
    }

    private double FinishReadingNumber()
    {
        StringBuilder builder = new StringBuilder();
        
        builder.Append((char)currentByte);
        
        
        char readByte;
        while (ReadNextByte())
        {
            readByte = (char)currentByte;
            
            // break if number ends
            if (readByte is < '0' or > '9' and not '.')
                break;
            
            builder.Append(readByte);
        }
        
        return double.Parse(builder.ToString());
    }

    private string FinishReadingString(char stringStartChar)
    {
        StringBuilder builder = new StringBuilder();
        
        int readByte = 0;
        int prevByte;
        while (ReadNextByte())
        {
            prevByte = readByte;
            readByte = currentByte;
            
            // break if string ends
            if (readByte == stringStartChar && prevByte != '\\')
                break;
            
            builder.Append((char)readByte);
        }
        
        return builder.ToString();
    }
}