# MetaPrograms

[![Build](https://img.shields.io/appveyor/ci/felix-b/metaprograms/master.svg)](https://ci.appveyor.com/project/felix-b/metaprograms)
[![Tests](https://img.shields.io/appveyor/tests/felix-b/metaprograms/master.svg)](https://ci.appveyor.com/project/felix-b/metaprograms)
[![Coverage](https://img.shields.io/codecov/c/github/nwheels-io/metaprograms/master.svg)](https://codecov.io/gh/nwheels-io/MetaPrograms)
[![Nuget](https://img.shields.io/nuget/vpre/MetaPrograms.svg)](http://www.nuget.org/packages/MetaPrograms/)

Enables **easy meta-programming** by providing **static analysis**, transformation, and **code generation** mechanisms in an **extensible** set of **programming languages**.

> **New to meta-programming?** it's a technique in which programs treat other programs as their data. It means a program can be designed to read other programs, transform them, and generate derived programs automatically. This allows programmers to minimize amount of code to express a solution, reducing the development time. [Read more on Wikipedia](https://en.wikipedia.org/wiki/Metaprogramming).

## Goals

This project aims to provide meta-programming infrastructure layer to full-stack _intentions-as-code_ frameworks based on .NET Core.

> **Intentions-as-code** is an approach, which applies meta-programming to full-stack development of SaaS and enterprise applications. It aims to let developers express the entire application, including non-functionals, with minimal amount of code on conceptual level (abstracted from concrete technology); then impelmentation details are generated per selected technology stacks. [Read more here](Docs/intentions-as-code.md).

## Languages and platforms

The framework itself is written in C# for .NET Core. It runs on Linux, macOS, and Windows.

The set of languages supported by the framework is extensible. Such extensibility is achieved by separating language-specific packages from the base package.

While the base package provides _universal code model_ for language-specific packages to use, the latter provide reading and/or writing capabilities in specific languages.

Currently, the following language-specific packages exist:

Language|Package|Read|Write|Remarks
---|---|---|---|---
C#|MetaPrograms.CSharp|V|V|based on Roslyn
JavaScript|MetaPrograms.JavaScript|X|V|

## Demo

The library passes POC integration test that demonstrates the main use case. [Read details here](Docs/poc.md).

## Status

The project is currently in early stage of development. You can play with the concept, but do not use it for production. Everything is subject to change, and no backward compatibility is maintained at this stage.  

## Build & Run

Requirements: [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/dotnet-core/2.1) (runs on Linux, macOS, and Windows)

```
$ git clone https://github.com/nwheels-io/MetaPrograms.git
$ cd Source
$ dotnet build
$ dotnet test IntegrationTests/MetaPrograms.IntegrationTests
```

## How it works (current concept)

![Concept illustration](Docs/concept-flow.png)

1. **Original artifacts** contain intentions expressed with minimal amount of concise code. In order to make understanding the intentions a simple task, they should be coded according to a defined convention, or on top of a domain-specific API.

1. **Language adapters** are packages that add capabilities of reading and writing a specific programming language. For instance, they are capable of parsing syntax and analyzing semantics of the code, producing _code model_ (explained next). 

1. **Universal code model** represents semantically bound structure of code. That is, identifiers in code are converted into references to model elements they identify. The code model is language-agnostic; it captures superset of modern programming languages features. 

1. **Meta-programming logic** is at the heart of the story. It is where you program transformations and generation of code models, which are then converted into code in target languages. Usually, you first capture domain-specific metadata out of original models; then based on the metadata, you manipulate and generate new models.

1. Resulting code models represent the final artifacts. Resulting models are either obtained by manipulating other models, or generated from scratch. Each model is created with a specific language in mind, as concrete language adapters only handle subset of the model that is applicable to the language.

1. Concrete language adapters generate code of the final artifacts from the code models in (5), in specific programming languages.

## Code Coverage

- dotnet tool install --global coverlet.console
- coverlet MetaPrograms.Tests/bin/Release/netcoreapp2.0/MetaPrograms.Tests.dll --target "dotnet" --targetargs "test MetaPr
ograms.Tests -c Release --no-build" --format opencover
- dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.0.0-rc11
- reportgenerator -reports:coverage.opencover.xml -targetdir:./coverage
