// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Parser;
using Parser.JsonValues;



string testJson = """
{
	"character": "va\u0072",
	"obj": {
		"randkey": "hello12345\"",
		"second": {
			"hi": 2
		}
	},
	"hundredIterator": [
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60
, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114,
 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199 
	],
	"tomb": [{
		"hello": "world",
		"multi": [{
			"multiProp": "hi"
		}]
	},
	{   
		"hello": "vilag"
	}],
	"iterator2": [{
		"heyJo": "this is a new iterator"
	}]
}
""";

Stream FromFile()
{
    string file = @"C:\Users\enbi8\Downloads\test.json";
    return File.OpenRead(file);
}


async Task<Stream> FromWeb()
{
    string link = "https://raw.githubusercontent.com/zemirco/sf-city-lots-json/master/citylots.json";
    var httpClient = new HttpClient();
    Stream response = await httpClient.GetStreamAsync(new Uri(link));
    return response;
}

Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(testJson));
//Stream stream = await FromWeb(); 
//Stream stream = FromFile();


/*
var jsonAnalyzer = new JsonAnalyzer(stream);
IJsonValue analyzed = jsonAnalyzer.Analyze();

var text = JsonSerializer.Serialize(analyzed, options: new JsonSerializerOptions
{
    WriteIndented = true
});
Print(text);

await Task.Delay(-1);


Print("Number of iterators: " + jsonAnalyzer.Iterators.Count());
foreach (ArrayValue iterator in jsonAnalyzer.Iterators)
{
    Print("Iterator: " + iterator.FullPath);
    PrintArray(iterator);
}


Print();
Print();

// print the full analyzed tree
if(analyzed is ObjectValue obj)
    PrintObject(obj);
*/



var parser = new JsonDocumentIterator(stream);
var rootElement = parser.Parse();
var obj = (ObjectValue) rootElement;

PrintObject(obj);

void Print(object? value = null)
{
    // comment this line to disable console output
    Console.WriteLine(value);
}

// print out an object's properties
void PrintObject(ObjectValue objValue)
{
    Print();
    foreach (var v in objValue.Properties)
    {
        PrintValue(v);
    }
}

// print out a default value
void PrintValue(IJsonValue value)
{
    Print("Name: " + value.Name);
    Print("Path: " + value.FullPath);
    Print("LookupPath: " + value.LookUpPath);
    Print("Type: " + value.Type);
    Print("Depth: " + value.Depth);
    Print("Member type: " + value.JsonMemberType);
    
    if(value is StringValue sv)
        Print("string value: " + sv.Value);
    else if(value is NumberValue nv)
        Print("number value: " + nv.Value);
    else if(value is BoolValue bv)
        Print("bool value: " + bv.Value);
    else
    {
        if(value is ObjectValue ov)
            PrintObject(ov);
        else if (value is ArrayValue av)
            PrintArray(av);

        return;
    }
    Print();
}

// print out an array
void PrintArray(ArrayValue arrayValue)
{
    Print();
    foreach (IJsonValue arrayValueValue in arrayValue.Values)
    {
        PrintValue(arrayValueValue);
    }
}

