Calculator
===============

A simple .NET library for parsing and evaluating mathematical expressions.

## Installation

Easily done if you have the dotnet CLI installed.

### Install from Nuget

```
dotnet add package calculator
```
### Build from source

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

## Usage

```C#
// Standard mathematical operations
var plus = new Operator('+', (a, b) => a + b, 0);
var subtract = new Operator('-', (a, b) => a - b, 0);
var multiply = new Operator('*', (a, b) => a * b, 1);
var divide = new Operator('/', (a, b) => {

    if (b == 0) {
        throw new Exception("Division by zero is not allowed");
    }

    return a / b;
}, 1);

// The natural logarithm
var ln = new Function("ln", arguments => {

    if (arguments.Count() != 1) {
        throw new Exception("The natural logarithm takes exactly 1 argument");
    }

    return Math.Log(arguments.First());
});

// Example multi-variable function
var mv = new Function("mv", arguments => {

    if (arguments.Count() != 2) {
        throw new Exception("'mv' takes exactly 2 arguments");
    }

    return arguments[0] + arguments[1];
});

// Example number constant
var pi = new NumberConstant("π", Math.PI);

var lexer = new Lexer();

var parser = new Parser(
    new[] { ln },
    new[] { plus, subtract, multiply, divide },
    new[] { pi }
);

var calculator = new Calculator(lexer, parser);

Console.WriteLine(calculator.GetResult("1+ln(π+2)/(mv(3,4)+5)")); // Outputs 1.1364469071880001
```