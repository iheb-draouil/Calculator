Calculator
===============

A simple .NET library for parsing and evaluating mathematical expressions.

# Installation

Easily done if you have the dotnet CLI installed.

## Install from Nuget

```
dotnet add package calculator
```

## Build from source

Build the project

```
dotnet publish --output <output-path>
```

Then reference the dll in your project's .csproj file

```html
<ItemGroup>
    <Reference Include="DllFile">
      <HintPath>Path\To\DllFile.dll</HintPath>
    </Reference>
</ItemGroup>
```