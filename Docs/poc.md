# Proof of concept 

<img src="concept-poc.png" align="right" />

Given intentions-as-code of a simplest web page, generate its front-end and backend implementations.
- Intentions as code: C# 7 on .NET Core 2
- Front-end: ES6 / JSX, a [Hyperapp](https://github.com/hyperapp/hyperapp) application
- Backend: C#, an [ASP.NET Core 2](https://github.com/aspnet/Home) application

### Web page flow

1. On the web page, user enters her name and clicks "Submit" button. 
1. The name is sent to GetGreetingForName operation in the backend 
1. The backend invokes Greeting service, which calculates greeting text for specified name.
1. Resulting greeting is sent back to the web page
1. Web page receives the response and displays the greeting. 

### Intentions as code

Here we use an imaginary domain-specific API (a kind of _domain-specific language_), which allows describing a web app in code.

Each page in web app is a class derived from `WebPage<TModel>`, where `TModel` is the _model_ (or the _state_) of the page. The only page in our POC is `HelloPage`:

```csharp
public class HelloPage : WebPage<HelloPage.HelloModel>
```

A page contains one or more _components_. Some components are built-in, while others are custom  (although the latter are not part of the POC). 

A component is included in a page by declaring read-only property like this:

```csharp
public FormComponent<HelloModel> Form => new FormComponent<HelloModel> {
    Model = Model
};
```

What have just happened:
1. We included a built-in component `FormComponent<TModel>` in our page
1. We named it `Form` (names within one page are unique)
1. We stated that it is bound to `HelloModel` (similarly to pages, every components is bound to a _model_). 
1. We bound component's `Model` property to page's `Model`, which means that `Form` is bound to the model of the entire page.

Models are classes enriched with semantics metadata. We explain semantics through C# attributes. The `HelloModel` class looks like this:

```csharp
public class HelloModel
{
    [PropertyContract.Validation.Required(AllowEmpty = false, MaxLength = 50)]
    public string Name { get; set; }

    [PropertyContract.I18n.Label("ServerSays")]
    public string Greeting { get; set; }
}
```

Now that we have a form and a model, and want to send data to server when the form is submitted. For that, we need to describe our backend API. We do it with a read-only property like this:

```csharp
public IGreetingService GreetingService => GetBackendApiProxy<IGreetingService>();
```

Function `GetBackendApiProxy<T>` is a marker API that explains our meta-programming logic that `GreetingService` property represents backend API.  `IGreetingService` is an application-defined interface in our example:

```csharp
public interface IGreetingService
{
    Task<string> GetGreetingForName(string name);
}
```

The last piece that's left to add is an intention to invoke `Greeting` service, pass it the `Name` from the model, and assign resulting greeting to the model. Here's how:

```csharp
public override void Controller()
{
    Form.Submitting += async () => Model.Greeting = await GreetingService.GetGreetingForName(Model.Name);
}
```

The `Controller` method is where we describe interactions between components, the page, and the backend. Here we attach handler to `Submitting` event of the `Form` component. As the name implies, this event is fired when the user submits the form. 

That's all. The full code listing is in [HelloPage.cs](https://github.com/felix-b/MetaPrograms/blob/master/Source/IntegrationTests/CSharpAndJavaScript/Example.App/HelloPage.cs)

## Packages

The following packages are part of the test

### Proof-of-concept packages

- Web app expressed through intentions-as-code - [Example.App](https://github.com/felix-b/MetaPrograms/tree/master/Source/IntegrationTests/CSharpAndJavaScript/Example.App), or go directly to [HelloPage class](https://github.com/felix-b/MetaPrograms/blob/master/Source/IntegrationTests/CSharpAndJavaScript/Example.App/HelloPage.cs).
- Imaginary web UI programming model (example of domain-specific API) - [Example.WebUIModel](https://github.com/felix-b/MetaPrograms/tree/master/Source/IntegrationTests/CSharpAndJavaScript/Example.WebUIModel)
- Sample backend generator, which translates UI model into an [ASP.NET Core](https://github.com/aspnet/Home) server application - [Example.AspNetAdapter](https://github.com/felix-b/MetaPrograms/tree/master/Source/IntegrationTests/CSharpAndJavaScript/Example.AspNetAdapter)
- Sample front-end generator, which translates UI model into a [Hyperapp](https://github.com/hyperapp/hyperapp) application - [Example.HyperappAdapter](https://github.com/felix-b/MetaPrograms/tree/master/Source/IntegrationTests/CSharpAndJavaScript/Example.HyperappAdapter)

### Production packages

- C# language adapter based on [Roslyn](https://github.com/dotnet/roslyn) framework - [MetaPrograms.Adapters.Roslyn](https://github.com/felix-b/MetaPrograms/tree/master/Source/MetaPrograms.Adapters.Roslyn)
- JavaScript / JSX language adapter - [MetaPrograms.Adapters.JavaScript](https://github.com/felix-b/MetaPrograms/tree/master/Source/MetaPrograms.Adapters.JavaScript)

### Integration test

- [ExampleAppImplementationTests](https://github.com/felix-b/MetaPrograms/blob/master/Source/IntegrationTests/MetaPrograms.IntegrationTests/CSharpAndJavaScript/ExampleAppImplementationTests.cs)
    - Front-end test case: `CanGenerateFrontEndImplementation`
    - Back-end test case: `CanGenerateBackEndImplementation`
    - [Expected outputs](https://github.com/felix-b/MetaPrograms/tree/master/Source/IntegrationTests/CSharpAndJavaScript/ExpectedOutput)
