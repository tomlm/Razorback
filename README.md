![https://github.com/tomlm/Razorback/blob/master/clipart0024.jpg](https://github.com/tomlm/Razorback/blob/master/clipart0024.jpg)

# Razorback

Razorback is a library which makes it easy to use ASP.NET Razor engine to convert XML templates into objects in your web server.



## Installation

1. Add razorback nuget to your .Asp.Net Core project

2. Update your Startup.cs to define that the IRazorbackTemplateEngine interface is implemented by the RazorbackTemplateEngine class.

```csharp
services.AddScoped<IRazorbackTemplateEngine, RazorbackTemplateEngine>();
```



## Define a model

Define a class which will act as a model, like this example:
```csharp
public class Person
    {
        public string Name { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public DateTime Birthday { get; set; }

        public List<KeyValuePair<string, string>> Factoids { get; set; } = new List<KeyValuePair<string, string>>();
}
```



## Creating a text template

You can create  text template by creating a new view as a .cshtml file
```csharp
@model RazorbackSample.Person 
Hello there, @Model.First @Model.Last
```



## Creating an XML template

You can create an object template for generating xml by creating a new view as a .cshtml file with XML syntax.
```xml
@model RazorbackSample.Person
@using System.Linq;
<?xml version="1.0" encoding="utf-16" ?>
<Card xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    <Version Major="1" Minor="0" />
    <Container>
        <TextBlock Size="Medium" Weight="Bolder">Hello world!</TextBlock>
        <ColumnSet>
            <Column Width="auto">
                <Image Size="Small" Style="Person" Url="http://bing.com/foo.png" />
            </Column>
            <Column Width="stretch">
                <TextBlock Weight="Bolder" Wrap="true">@Model.First @Model.Last</TextBlock>
            </Column>
        </ColumnSet>
    </Container>
    @if (Model.Factoids.Any())
    {
        <Container>
            <FactSet>
                @foreach (var factoid in Model.Factoids)
                {
                    <Fact Title="@factoid.Key" Value="@factoid.Value" />
                }
            </FactSet>
        </Container>
    }
</Card>
```


## Getting access to the razorback instance

Your razorback instance is defined as a depency injected object in startup.cs. To get access to it, you simply change your controller constructor to refer to it and save it away like this:

```csharp
private readonly IRazorbackTemplateEngine _razorback;

public ValuesController(IRazorbackTemplateEngine razorback)
{
	_razorback = razorback;
}
```



## Binding to your text  template

When you want to bind an instance of your Person model to your text template you can simply do this:
```csharp
var text = await razorback.BindToText<Person>("Values/Text", model);
```



## Binding to your card template

When you want to bind an instance of your Person model to your xml template you do that by calling BindXmlToObject<ModelT, OutputT>(...) where 

* **ModelT** is the type of the model that you are binding from 
* **OutputT** is the type of the object you want the xml deserialized into  

This looks like this:
```csharp
var card = await this._razorback.BindXmlToObject<Person, AdaptiveCard>("Values/Card", model);
```


