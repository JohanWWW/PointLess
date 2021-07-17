# ZeroPoint (Programming Language)
ZeroPoint is a dynamic and object oriented programming language built in C#.

## Examples
### Variable Declarations
```
a = 12;
b = 6;
c = a + b;
println(c);

a = "Hello";
b = "World";
c = a + b;
println(c);
```

Output:
```
18
HelloWorld
```

### Object Definitions
Objects in ZeroPoint are anonymous and are really simple to define. Below is an example of an object being assigned to a variable.
```
person = {
    firstName = "Björn",
    lastName = "Björnsson"
};
```

### Function Definitions
```
// Consumer definition
repeat = (message, count) => {
    i = 0;
    while (i < count) {
        println(message);
        i += 1;
    }
};

// Consumer call
repeat("Hello World!", 3);
```

Output:
```
Hello World!
Hello World!
Hello World!
```

### Recursion
```
factorial = (x) => {
    result = null;
    if (x == 1) {
        result = x;
    }
    else {
        result = x * factorial(x - 1);
    }
    return result;
};

println(factorial(5));
```

Output:
```
120
```

### Encapsulation
Although classes are non-existent and more of a pattern in ZeroPoint it is still possible to simulate classes with functions which also allows for encapsulation.
```
Person = (firstName, lastName, age) => {
    _getAge = () => {
        return age;
    };

    _ageUp = () => {
        age += 1;
    };

    _getFullName = () => {
        return firstName + " " + lastName;
    };

    // Returns the constructed object which references private variables!
    return {
        firstName = firstName,
        lastName = lastName,
        getAge = _getAge,
        ageUp = _ageUp,
        getFullName = _getFullName
    };
};
```

### Error Handling
ZeroPoint support try-catch and throw statements as many other languages do.
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
    println(q);
}
catch e => {
    println("Following error occurred: " + e);
}
```

Output:
```
Following error occurred: Cannot divide by zero!
```

## CLI Installation
.NET 5 SDK is recommended. Not tested on earlier .NET releases.

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

