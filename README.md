# Tariff Comparison Console App - Trevor Reeves

## Overview

The application is written in C#, and it runs on the .NET Core CLR, which of course runs on OSX and Linux.

## Compile and Run

### Install .NET Core SDK

Visit https://www.microsoft.com/net/learn/get-started/macos and choose your OS to download the SDK.

### Compile

Once the SDK is installed, cd to the root of the repo, and 

    dotnet build

...will build the app.

    dotnet test
    
...will run all the tests.

### Run

To build and run in dev mode, but this has a slow start up time...

    cd src/Tariffs.Console
    dotnet run cost 2000 2300

Or to run normally, first 'publish', then run...

    cd src/Tariffs.Console
    dotnet publish

    cd bin\Debug\netcoreapp2.0\publish\
    dotnet .\Tariffs.Console.dll cost 2000 2300

## Implementation Notes

I've tried my best to stay functional wherever possible.  All classes are immutable and hold only data, all collections, although are of mutable concrete implementations are only exposed through 'read only' interfaces.  There is only really one variable that is global mutable state, which is the Tariff dictionary in Tariffs.Data.TariffSource, and this is updated only via an atomic thread-safe mechanism when new data is available.

I tried to stay clear of using many third party libraries - Newtonsoft.Json is used to deserialize JSON, and Microsoft.Extensions.DependencyInjection (DI container) is used to do a small amount of DI at the start, though I tried to avoid building the whole app based on it.

### VNext

Given more time, of course I would add unit test coverage for Tariffs.CommandLine and Tariffs.Console - all these are unit testable. Also it would be trivial to add some automated end-to-end or integration tests that test all the components combined, either with  the json file implementation faked with an in memory, or even hitting disk and loading in the data from the file.

Behaviour wise it would be simple to extend the CommandLine component to support an interactive (or REPL) mode, and to implement a file watcher that emits events to which the file loader was subscribed, to refresh the Tariff data whenever it was updated. The same pattern would apply if the tariffs were stored in a db and other application components were notifying the app of changes.

### Assemblies

*Tariffs.Console*

Defines the entry point for the application, with the composition root of the DI container, and currently has the implementation for the 2 commands that are available.

*Tariffs.CommandLine*

A small component to allow multiple commands to be defined in an application, and to parse command line parameters and exit in an appropriate manner.

*Tariffs.Calc*

Contains the domain logic for calculating fuel costs and fuel usages.

*Tariffs.Data*

Defines the domain model and in-memory storage abstraction for tariffs.

*Tariffs.Data.SimpleFile*

Provides the mechanism for loading tariffs from a json file.

