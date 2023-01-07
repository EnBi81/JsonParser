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
        while (!shouldReadNextByte || ReadNextByte())
        {
            shouldReadNextByte = true;
            
            int nextByte = currentByte;
            CurrentValue = null;
            
            // skip the whitespace and unnecessary characters
            if(nextByte is ' ' or '\t' or '\r' or '\n' or ':' or ',')
                continue;

            // object
            if (nextByte == '{')
            {
                CurrentToken = JsonReadToken.StartObject;
                return true;
            }

            if (nextByte == '}')
            {
                CurrentToken = JsonReadToken.EndObject;
                return true;
            }
            
            // array
            if (nextByte == '[')
            {
                CurrentToken = JsonReadToken.StartArray;
                return true;
            }
            
            if (nextByte == ']')
            {
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
        var builder = new StringBuilder();
        
        builder.Append((char)currentByte);


        while (ReadNextByte())
        {
            var readByte = (char)currentByte;
            
            // break if number ends
            if (readByte is < '0' or > '9' and not '.')
                break;
            
            builder.Append(readByte);
        }
        
        return double.Parse(builder.ToString());
    }

    private string FinishReadingString(char stringStartChar)
    {
        var builder = new StringBuilder();

        while (ReadNextByte())
        {
            int readByte = currentByte;

            // if the current byte is a start of an escape character, read the next character as well
            if (currentByte is '\\')
            {
                if (!ReadNextByte())
                    throw new Exception("Unexpected end of stream");

                // https://www.tutorialspoint.com/json_simple/json_simple_escape_characters.htm
                // https://www.crockford.com/mckeeman.html
                readByte = currentByte switch
                {
                    'b' => '\b',
                    'f' => '\f',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '\"' => '\"',
                    '\\' => '\\',
                    'u' => FinishReadingEscapeUnicode(),
                    _ => throw new ArgumentException("Invalid escape character: \\" + (char)currentByte)
                };
            }
            // break if string ends
            else if (readByte == stringStartChar)
                break;
            
            builder.Append((char)readByte);
        }
        
        return builder.ToString();
    }

    /// <summary>
    /// 'U' must be read beforehand
    /// </summary>
    /// <returns></returns>
    private char FinishReadingEscapeUnicode()
    {
        StringBuilder builder = new ();
        for (int i = 0; i < 4; i++)
        {
            ReadNextByte(); // read the 4 characters in
            builder.Append((char)currentByte);
        }
        
        // convert text to hexadecimal number
        var arr = Convert.FromHexString(builder.ToString()); 
        // convert the two return bytes into one character
        return (char)((arr[0] << 8) | arr[1]); 
    }
}