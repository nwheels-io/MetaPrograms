MetaPrograms
=====

[![Build](https://img.shields.io/appveyor/ci/felix-b/metaprograms/master.svg)](https://ci.appveyor.com/project/felix-b/metaprograms)
[![Tests](https://img.shields.io/appveyor/tests/felix-b/metaprograms/master.svg)](https://ci.appveyor.com/project/felix-b/metaprograms)
[![Coverage](https://img.shields.io/codecov/c/github/nwheels-io/metaprograms/master.svg)](https://codecov.io/gh/nwheels-io/MetaPrograms)
[![Nuget](https://img.shields.io/nuget/vpre/MetaPrograms.svg)](http://www.nuget.org/packages/MetaPrograms/)

Enables **easy meta-programming** by providing **static analysis**, transformation, and **code generation** mechanisms in an **extensible** set of **programming languages**.

> **New to meta-programming?** it's a technique in which programs treat other programs as their data. It means, a program can be designed to read other programs, transform them, and generate derived programs automatically. This allows programmers to minimize amount of code to express a solution, reducing the development time. [Read more on Wikipedia](https://en.wikipedia.org/wiki/Metaprogramming).

<img align="right" width="367" height="463" src="Docs/concept-flow-narrow.png">

## Goal

Provide meta-programming infrastructure layer to full-stack _intentions-as-code_ frameworks based on .NET Core.

> **Intentions-as-code** is an approach, which applies meta-programming to full-stack development of SaaS and enterprise applications. It aims to let developers express the entire application, including non-functionals, with minimal amount of code on conceptual level (abstracted from concrete technology); then impelmentation details are generated per selected technology stacks. [Read more here](Docs/intentions-as-code.md).

## Status

> WARNING: In early stages of development. Good for experimenting with the concept, but not for production. No backward compatibility is matintained at this time. 

## Demo

<img src="Docs/concept-poc.png" align="left" />

Requirements: [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/dotnet-core/2.1) (runs on Linux, macOS, and Windows)

### What's in the demo

- [Example.WebUIModel](Source/TestCases/CSharpAndJavaScript/Example.WebUIModel): a sample **programming model** of single-page web apps 
- [Example.HyperappAdapter](Source/TestCases/CSharpAndJavaScript/Example.HyperappAdapter): a sample **technology adapter** that generates JavaScript web app based on the [Hyperapp](https://github.com/hyperapp/hyperapp) framework 
- [Example.AspNetAdapter](Source/TestCases/CSharpAndJavaScript/Example.AspNetAdapter): a sample **technology adapter** that generates C# backend service based on [ASP.NET Core](https://github.com/aspnet/Home) Web API.
- [Example.App](Source/TestCases/CSharpAndJavaScript/Example.App): intentions-of-code (in C#) of a sample web app, including some simple backend logic. You may also write your own intentions-as-code project, and run it with the demo (explained in the **Getting Started** section).

For more details on the above modules, read about [proof-of-conept integration test](Docs/poc.md).

### How to run

```
$ git clone https://github.com/nwheels-io/MetaPrograms.git
$ cd MetaPrograms/Source/Demo
$ dotnet build
$ dotnet run
```

### What to expect

Once the run completes, the generated projects will be placwd in the `DemoResults` subfolder:
- `DemoResults/FrontEnd` will contain an **npm** project of Hyperapp web app (client side)
- `DemoResults/BackEnd` will contain **.NET Core** project of ASP.NET Core Web API (server side)

## Languages and platforms

**MetaPrograms** is developed in C# for .NET Core, and runs on Linux, macOS, and Windows. 

The set of languages supported by the framework is extensible. Also, there is no limit on platforms, frameworks, and products that can be targeted by the generated code.

In order to analyze and generate code in specific languages, corresponding language-specific packages must be used in addition to the base **MetaPrograms** package. 

The following languages are currently supported:

Language|Package|Analysis|Generation|Remarks
---|---|---|---|---
C#|[MetaPrograms.CSharp](Source/MetaPrograms.CSharp)|V|V|based on [Roslyn](https://github.com/dotnet/roslyn)
JavaScript|[MetaPrograms.JavaScript](Source/MetaPrograms.JavaScript)|X|V|ES6 + JSX

The roadmap for this list is to include a dozen of languages. Note that not every package supports both the analysis and the generation: some packages only support one of the two.

## Getting started

This section assumes you have cloned and built the repository, as explained in the **Demo** section. If you haven't done that yet:

```
$ git clone https://github.com/nwheels-io/MetaPrograms.git
$ cd MetaPrograms/Source/Demo
$ dotnet build
```

In the following instructions, `<clone_folder>` is the folder where you cloned the repository. 

### Demo with your own intentions-as-code

You can use any development environment that works with .NET Core (Visual Studio 2017, Visual Studio Code, or Rider).

- create a new class library project that targets `netstandard2.0`
- add a reference to `Example.WebUIModel` project, located at the path: `<clone_folder>/Source/TestCases/CSharpAndJavaScript/Example.WebUIModel/Example.WebUIModel.csproj` 
- create a page class; to get the idea of how to do it, look at [HelloPage.cs](Source/TestCases/CSharpAndJavaScript/Example.App/HelloPage.cs) from the demo
  - note that the demo supports exactly one page class
- create your own view model with properties you want
  - note that the demo only supports flat view models (no nested objects or collections)
- create any number of backend services, like this: [IGreetingService.cs](Source/TestCases/CSharpAndJavaScript/Example.App/Services/IGreetingService.cs)

Make sure your project successfully builds, then:

- in terminal, go to folder `<clone_folder>/Source/Demo` 
- run (replace `/path/to/your/project` with the path to fodler where your project is located):
  ```
  $ dotnet run -- /path/to/your/project
  ```
- the results will be placed in the `<clone_folder>/Source/Demo/DemoResults` folder, the same as explained in the **Demo** section.

### Writing your own programming model

TBD

### Writing technology adapters for your programming model

TBD

## Contributing

TBD
