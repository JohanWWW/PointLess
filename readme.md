# Zeropoint (Programming Language)
ZP is a dynamic and object oriented programming language built in C#.

## Examples

### Object Definitions
Objects in ZP are anonymous reference types and are really simple to define. Below is an example of an object being assigned to the variable `myObject`.
```
myObject = {
    identifier = "abc123",
    value = 5.0898989,
    timestamp = 10330278590000,
    
    // Inner object
    position = {
        column = 15,
        row = 3
    }
};
```

### Method Definitions
```
action = () => {
    Console.println("action was called");
};

function = (a, b) => {
    return a + b;
};

consumer = (x) => {
    Console.println("result: " + x);
};

provider = () => {
    return 5.98;
};

lambda = x => x * 2;
```
```
repeat = (message, count) => {
    i = 0;
    while (i < count) {
        Console.println(message);
        i += 1;
    }
};

repeat("Hello World!", 3);
```

Output:
```
Hello World!
Hello World!
Hello World!
```

### Encapsulation
Although classes are non-existent and more of a pattern in ZP it is still possible to simulate classes with functions which also allows for encapsulation. In the example below `firstName` and `lastName` become private and readonly values.
```
Person = (firstName, lastName) => {

    getFirstName = () => firstName;

    getLastName = () => lastName;

    getFullName = () => getFirstName() + " " + getLastName();

    // Construct and return the object
    return {
        getFirstName = getFirstName,
        getLastName = getLastName,
        getFullName = getFullName
    };
};
```

### Method Overloading
Methods in ZP are overloadable. Just use the `+=` operator or add two or more method statements together with the `+` operator and assign to variable. The method overrides must differ in parameter count.
```
sayHello = name => Console.println("Hello " + name + "!");

// Default behaviour
sayHello += () => sayHello("World");
```

### Operator Function Overriding
Not only can you perform arithmetic operations between two numbers or strings. You can even perform operations between two objects or rather an object with a value of any type you wish, and you decide what should be returned from that operation. This is possible because operators in ZP are overridable functions that take two arguments and return a value. These operator functions are automatically called by the interpreter during runtime whenever corresponding operator is used. An example is ZP framework's complexMath library which makes use of operator overriding. The value returned by `complex` is an object:

```
use std;
use complexMath;

complex = ComplexMath.Complex;
println = Console.println;

main = (args) => {
    a = complex(0.0, 1.0);
    b = complex(5.0, 10.0);

    // The objects can be used like numbers!
    sum = a + b;
    difference = a - b;
    product = a * b;
    quotient = a / b;

    println(a + " + " + b + " = " + sum);
    println(a + " - " + b + " = " + difference);
    println(a + " * " + b + " = " + product);
    println(a + " / " + b + " = " + quotient);
    println();
    println(product + " * conj" + product + " = " + (product * ComplexMath.conjugate(product)));
    println("í^2 == -1 = " + ((complex(0.0, 1.0) * complex(0.0, 1.0)) == -complex(1.0, 0.0)));
};
```

Output:
```
(í) + (5+10í) = (5+11í)
(í) - (5+10í) = (-5-9í)
(í) * (5+10í) = (-10+5í)
(í) / (5+10í) = (0.08+0.04í)

(-10+5í) * conj(-10+5í) = (125)
í^2 == -1 = true
```

### Error Handling
ZP support try-catch and throw statements as many other languages do.
The catch part of the statement is a type of consumer function that accepts anything as an argument whether it is a string, integer, object etc. and is invoked whenever a throw statement is reached.
```
divide = (x, y) => {
    if (y == 0) {
        throw "Cannot divide by zero!";
    }
    return x / y;
};

try {
    q = divide(5, 0);
    Console.println(q);
}
catch e => {
    Console.println("Following error occurred: " + e);
}
```

Output:
```
Following error occurred: Cannot divide by zero!
```

### Error & Exception Messages
ZP's parsing engine can detect syntax errors and can tell you exactly where a syntax error is detected.
```
Syntax error at C:\Users\me\project\source\program.0p:140:27
------------------------------------------------------------
        someValue = obj.getValue());
                                  ^
                                  Unexpected proceeding token
```

ZP's interpreter catches exceptions and can also tell you where these occurred and what went wrong.
```
Unhandled runtime exception
---------------------------
        product = 2 * enumerator.next();
Cannot access members on void type $enumerator
        at C:\Users\me\project\source\program.0p:148:15
```

## CLI Installation

### Prerequisties
[.NET 5 SDK](https://dotnet.microsoft.com/download) is required in order to install and run this project. Even though it is not tested, it might work on earlier .NET releases.

- Clone this repository and place it anywhere on your system.

### Windows Cmd
- Navigate to the project named *CLI* with the `cd` command. \
For example `cd <path>\ZeroPoint\CLI` *(replace \<path\> with appropiate path)*
- Run the following commands in the command prompt to generate nuget package and install the command line interface
    - `dotnet pack`
    - `dotnet tool install --global --add-source .\nupkg zeropointcli`

### macOS/Linux Terminal
- Navigate to the project named *CLI* with the `cd` command. \
For example `cd <path>/ZeroPoint/CLI` *(replace \<path\> with the appropiate path)*
- Run the following commands in Terminal to generate nuget package and install the command line interface
    - `dotnet pack`
    - `dotnet tool install --global --add-source ./nupkg zeropointcli`

## Uninstall CLI
To uninstall the command line interface run the following command \
`dotnet tool uninstall --global zeropointcli`


## Create and run project
- Create a new project by creating a new directory anywhere on your system and navigate to it in Cmd/Terminal 
- Run following command `zero new` to generate project files such as configurations, libraries and boilerplate code

The generated files will appear in the project directory.
Open the file named *program.0p* which is located in *source* and begin coding!
To run the program, execute `zero run`.

By default, the program's entry point is the function named *main*. You can change this at any time by modifying the project file.
A program can consist of code that is split into multiple `.0p` files. If a new file is added to the project it needs to be manually included in the project file. The filename (extension omitted) also needs to be listed somewhere in the compile order list. The file that is listed first in the compile order is compiled first. The file that contains the program's entry point should always be the last to be compiled. The compile order needs to be ordered properly in order to avoid referencing problems. Reference code from another file with the `use <lib_name>;` keyword. The `use` statement should be placed at the top the referencer file.


## Documentation
See [zeropoint cheatsheet](cheatsheet.md). Documentation for framework code exists in the framework's source code.
