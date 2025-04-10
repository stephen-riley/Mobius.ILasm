# BigCow.Mobius.ILasm

This repo is a fork of [Mobius.ILasm](https://github.com/kkokosa/Mobius.ILasm). It's been reconfigured to be a `net8.0` package that builds without warnings. For background and usage, please keep reading.

![Build and test](https://github.com/stephen-riley/Mobius.ILasm/workflows/Build/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/Mobius.ILasm)](https://www.nuget.org/packages/BigCow.Mobius.ILasm)
[![Downloads](https://img.shields.io/nuget/dt/BigCow.Mobius.ILasm)](https://www.nuget.org/packages/BigCow.Mobius.ILasm/)
[![Stars](https://img.shields.io/github/stars/stephen-riley/Mobius.ILasm)](https://github.com/stephen-riley/Mobius.ILasm/stargazers)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

---

## Mobius.ILasm

Common Intermediate Language (CIL) assembler available as a library and command-line tool (like `ilasm`). It is based on a CIL assembler code taken out from [the Mono runtime](https://github.com/mono/mono), including the CIL parser autogenerated by [the Jay tool](https://github.com/mono/mono/tree/main/mcs/jay) (which in turn was a C# port of a Berkeley Yacc parser for Java).

Current refactorings include:

-   main class `Driver` usage change
-   using `MemoryStream` instead of files directly
-   error reporting
-   initial unit tests
-   CLI runner extracted and command line parsing
-   logger abstraction
-   minor renaming

This is part of [Mobius](https://tooslowexception.com/mobius-net-runtime-running-on-net-core/) project - experimental .NET runtime running on .NET Core.

## Library

Definitely the most useful use case, as it is the first available .NET library that you can use to assemble textual CIL files.

Currently it targets .NET Standard 2.0 and is also available as Mobius.ILasm NuGet package.

### Example usage

```cs
// Read some CIL code
var cil = File.ReadAllText(@"./trivial/helloworldconsole.il");

var logger = new Logger();
var driver = new Driver(logger, Driver.Target.Dll, showParser: false, debuggingInfo: false, showTokens: false);

// Assemble
using var memoryStream = new MemoryStream();
driver.Assemble(new [] { cil }, memoryStream);
memoryStream.Seek(0, SeekOrigin.Begin);

// Load assembled binary stream into AsemblyLoadContext and execute
var assemblyContext = new AssemblyLoadContext(null);
var assembly = assemblyContext.LoadFromStream(memoryStream);
var entryPoint = assembly.EntryPoint;
var result = entryPoint?.Invoke(null, new object[] { new string[] { } });
```

_Note: Due to the early stage of development, and some legacy code improvements, the available API is subject to change in future versions._

## Command line tool

Typical usage is just to assembly a spefied `il` file:

```bash
> `mobius.ilasm.cil -I helloworld.il`
```

which will produce `helloworld.dll` that you can execute by `dotnet` command (if your provide `hellologger.runtimeconfig.json`). Additionaly, for Windows, you can produce EXE file that will be self-executable.

All options available:

```bash
InputFile* (-I)             IL file to be parsed
OutputFile (-O)
Target (-T)                 [Default='Dll']
                            Dll
                            Exe
NoAutoInherit (-nai)
Debug (-D)                  [Default='False']
ShowParser (-sp)            [Default='False']
ShowTokens (-st)            [Default='False']
StrongKeyFile (-S)          Strongname using the specified key file
StrongKeyContainer (-Str)   Strongname using the specified key container
```
