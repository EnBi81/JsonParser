// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Parser;
using Parser.JsonValues;

var file = @"C:\Users\enbi8\Downloads\test.json";
var largeLink = "https://raw.githubusercontent.com/zemirco/sf-city-lots-json/master/citylots.json";

Stream FromFile(string file)
{
    return File.OpenRead(file);
}

async Task<Stream> FromWeb(string link)
{
    HttpClient httpClient = new HttpClient();
    var response = await httpClient.GetStreamAsync(new Uri(link));
    return response;
}

//var stream = await FromWeb(largeLink); 
var stream = FromFile(file);

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

