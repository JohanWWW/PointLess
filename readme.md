# Pointless (Programming Language)
Pointless is a dynamic and object oriented programming language.

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
Objects in Pointless are anonymous and are really simple to define. Below is an example of an object being assigned to a variable.
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
        i = i + 1;
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

Although classes are non-existent in Pointless it is still possible to simulate class definitions with functions which allows for encapsulation.
```
// Class-like function with constructor parameters
Person = (firstName, lastName, age) => {
    _getAge = () => {
        return age;
    };

    _ageUp = () => {
        age = age + 1;
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

## CLI Installation
.NET 5.0 SDK is recommended. Not tested on earlier .NET releases. Not tested on Mac.

- Clone this repository
- Launch 'Developer Command Prompt for VS' and set current directory to the repository's path
- Set the current directory the CLI-project: `cd CLI`
- Strongly typed resource classes needs to be generated in order for the project template engine to work. To do this you can use ResGen. In the developer command prompt run the following commands \
`resgen /useSourcePath /str:csharp,ProjectGeneration ProjectGeneration\ProjectFiles.resx` \
`resgen /useSourcePath /str:csharp,ProjectGeneration ProjectGeneration\StandardLibraries.resx`
- Run the following command in the developer command prompt to generate nuget package \
`dotnet pack`
- Install the command line interface \
`dotnet tool install --global --add-source .\nupkg pointlesscli`

## Uninstall CLI
To uninstall the command line interface run the following command \
`dotnet tool uninstall --global pointlesscli`


## Get Started
- Create a new project by creating a new directory anywhere on your system and open in Visual Studio Code
- In vs code, launch a new instance of Windows PowerShell (or cmd) and run `pointless init`. This command will generate project files such as configurations, libraries and boilerplate code

The generated files will appear in the project directory.
Open the file named 'program.ptls' which is located in 'source' and begin coding! \
By default, the program's entry point is the function named 'main'. You can change this by modifying the project file.