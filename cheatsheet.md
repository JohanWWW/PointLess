# Zeropoint Cheat Sheet

## Import code from another file
To import custom code it has to be included and assigned a compile order in the project file `project.json`.
```
use <id>;
```
`<id>` The name of the file.

## Conditional Statements
```
if (<boolean expression>) {
    <code>
}
else if (<boolean expression>) {
    <code>
}
else {
    <code>
}
```
`<boolean expression>` is an expression that evaluates to a boolean, in other words `true` or `false`. The `else if` and `else` clauses are optional. A conditional statement can have multiple `else if` clauses.
`<code>` is the program to be executed if its clases condition evaluates to true.

## Loops
### While Loop
```
while (<boolean expression>) {
    <code>
}
```
`<boolean expression>` is an expression that evaluates to a boolean. \
`<code>` is the program to be executed while the boolean expression evaluates to `true`.

### Foreach Loop
```
foreach (<id> in <enumerable>) {
    <code>
}
```
`<id>` is a variable of an element from the specified enumerable. \
`<enumerable>` is an arbitrary collection. \
`<code>` is the program to be executed for each element in the enumerable. \
The foreach loop iterates through the collection, assigning elements to `<id>` for each iteration.
Note that foreach-loops only works on objects that correctly implement the `enumerator` method. All major collection in ZP framework including the built in collection types are enumerable.

## Error Handling
### Throw Statement
```
throw <expression>;
```
`<expression>` is an arbitrary expression provided as an argument to the throw statement.

### Try-Catch Statement
```
try {
    <code1>
}
catch <arg> => {
    <code2>
}
```
`<code1>` is the program to catch any exceptions that may be thrown. \
`<code2>` is the program to be executed if `<code1>` threw an exception. \
`<arg>` is the argument that was provided from a throw statement.

## Variable Declaration & Assignment
```
<id> = <expression>;
```
`<id>` is the identifier of a variable to declared. If the variable already exists, it is overwritten with the new value. \
`<expression>` is the value to be assigned to the variable.

### Assignment Operators (`<op>=`)
Extensions of the ' `=` ' operator. The following operators only works on defined variables and are equivalent to: `<id> = <id> <op> <expression>;` where `<op>` is a binary operator. Following list is unordered.

- `+=` addition assignment
- `-=` subtraction assignment
- `*=` multiplication assignment
- `/=` division assignment
- `%=` modulo assignment
- `&=` (bitwise|logical) AND assignment
- `^=` (bitwise|logical) XOR assignment
- `|=` (bitwise|logical) OR assignment
- `<<=` left shift assignment
- `>>=` right shift assignment

## Operators
Different flavours of operators. The majority of ZP's operators are overridable.

### Binary Operators
Usage `<expression> <op> <expression>` where `<op>` is a binary operator. Operations that are performed by the binary operators varies depending on the type of the operands and their operator implementations. Following operators are grouped by precedence.

Multiplicative operators
- `*` multiplication
- `/` division
- `%` modulo

Additive operators
- `+` addition
- `-` subtraction

Bitwise shift operators
- `<<` shift left
- `>>` shift right

Relational operators
- `<` less than
- `>` greater than
- `<=` less than or equal
- `>=` greater than or equal

Equality operators
- `==` equal
- `!=` not equal
- `===` strict equal
    - Compares two operands and returns true if the operands are of the same type and equal, otherwise return false
    - This operator is not overridable
- `!==` strict not equal
    - Compares two operands and returns true if the operands are not of the same type or not equal
    - This operator is not overridable

Bitwise AND `&`

(Bitwise|Logical) XOR `^`

Bitwise OR `|`

Logical AND `&&`

Logical OR `||`

### Unary Operators
Usage `<op><expression>` where `<op>` is a unary operator.

- `!` logical negation
- `-` arithmetic negation

### Ternary Operators

#### Ternary conditional operator `?:`
Usage `<boolean expression> ? <expression1> : <expression2>` where `<boolean expression>` is an expression that evaluates to a boolean, `<expression1>` is an expression that is evaluated if the boolean expression is true, `<expression2>` is an expression that is evaluated if the boolean expression is false.

## Methods
Methods that are part of ZP's method group are:
### Function
Functions have one or more parameters and returns a value.
```
<id> = (<params>) => {
    <code>
    return <expression>;
};
```
`<id>` The name of the function. \
`<params>` One or more comma separated parameters. \
`<code>` The program to be executed when calling the function. \
`<expression>` The return value of the function.

### Consumer
Consumers have one or more parameters and does not return a value.
```
<id> = (<params>) => {
    <code>
};
```
`<id>` The name of the consumer. \
`<params>` One or more comma separated parameters. \
`<code>` The program to be executed when calling the consumer.

### Action
Actions do not have any parameters and does not return a value.
```
<id> = () => {
    <code>
};
```
`<id>` The name of the action. \
`<code>` The program to be executed when calling the action.

### Provider
```
<id> = () => {
    <code>
    return <expression>;
};
```
`<id>` The name of the provider. \
`<code>` The program to be executed when calling the provider.
`<expression>` The return value of the provider.

Methods can also be created with a lambda expression but is evaluated to one of the method types above.
```
<id> = (<params>) => <expression>;
```

All of the method types above are overloadable. Too add an overload to an existing method, use the addition assignment (`+=`) operator as below:
```
<id> += <method>;
```
`<method>` An additional method.

Note that the method overload must differ from existing overloads in parameter count.

## Special Methods
### Operator Functions
In ZP operators are functions that are overridable. Operator functions can only be implemented for objects. Operator functions are implemented in the body of an object. \
Implementing a binary operator requires three parameters:
```
operator <left id> [<op>] <right id> => {
    <code>
    return <expression>;
}
```
`<left id>` Represents the value on the left side of the operator which is the object itself. \
`<op>` The operator to override. \
`<right id>` Represent the value on the right side of the operator. \
Overriding an operator also overrides its assignment operator equivalent (`<op>=`).

Overloadable binary operators: `+`, `-`, `*`, `/`, `%`, `==`, `!=`, `&&`, `||`, `^`, `&`, `|`

-------------------------------------------------------------------------------------------------------------------

Implementing a unary operator that is placed on the left side of an expression or variable requires two parameters:
```
operator unary [<op>]<right id> => {
    <code>
    return <expression>;
}
```
`<op>` The operator to override. \
`<right id>` Represents the value to perform the operation on which would be the object itself.

Overloadable unary operators: `!`, `-`


Operators in ZP are not overloadable and there can only exist one override per operator in the context of an object.

--------------------------------------------------------------------------------------------------------------------

### Indexer Methods
Indexer is a convenient way of accessing data of an array. In ZP indexers are methods that can be implemented for objects. Indexer methods can only be implemented for objects and are implemented in the body of an object. Indexers are overloadable. \
Implementing a getter indexer requires at least one parameter:
```
indexer [<params>] => {
    <code>
    return <expression>;
}
```
`<params>` One or more comma separated parameters.

Implementing a setter indexer requires at least two parameters and does not have a return value:
```
indexer [<params>] <- <param> => {
    <code>
}
```
`<params>` One or more comma separated parameters. \
`<param>` The value on the right side of the ' `=` ' operator.

To overload an indexer method, just add an additional indexer method that differs in parameter count.

## Native Methods
Native methods are methods that references C# code and are accessed through an API. These methods requires the `native` in the beginning of the definition and an inject statement after the lambda arrow.

### Native Function
```
<id> = native (<params>) => <@"<implementation id>">;
```

### Native Provider
```
<id> = native () => <@"<implementation id>">;
```
`<impl id>` Implementation identifier of the C# implemented method.


## Built-In Data Types
### Integer
Integers are arbitrarily sized (big integer). There are three different ways to define an integer. Integer is a built in value type.
```
// Base 10
val = 128;
val = -128;

// Base 2 (binary)
val = 0b10000000;

// Base 16 (hex)
val = 0x80;
```

### Decimal
Decimals are arbitrarily precisioned (big decimal). Decimal is a built in value type.
```
val = 0.123456789;
val = .123456789;
val = -0.123456789;
val = -.123456789;
```

### Byte
Bytes are unsigned 8-bit (1 byte) values. A byte can be represented as a number between 0 and 255. There are three different ways to allocate a byte. Byte is a built in value type.
```
// Base 10
val = b'255;

// Base 2 (binary)
val = b'0b11111111;

// Base 16 (hex)
val = b'0xFF;

```

### Character
A characters use format UTF-32 and takes up 4 bytes in memory. There are two different ways to define a character. Only one character or escape sequence may be present in the literal. Character is a built in value type.
```
// Assign character z to variable
character = 'z';

// Assign character å to variable with escape sequence
character = '\xE5';
```

### Boolean
A boolean represents a single bit in memory that can either be 0 or 1 or more commonly known as `true` or `false`. Boolean is a built in value type.
```
yes = true;
no = false;
```

### String
Strings are a sequence of UTF-32 characters. String is a built in reference type.
```
text = "Hello World!";
```
A string evaluates to a string object that contains members.
#### Methods
- `length()->int`
    - Returns the length of the string
- `toUpper()->string`
    - Returns an uppercased version of the string
- `toLower()->string`
    - Returns a lowercased version of the string
- `split(separator:(string|char))->string[]`
    - Arguments:
        - Separator: A string or a character
    - Returns an array of strings that were split on the specified separator.
- `replaceAll(oldString:string, newString:string)->string`
    - Arguments:
        - oldString: The string to replace
        - newString: The replacement string
    - Returns a string with replaced substrings
- `substring(start:number, end:number)->string`
    - Arguments:
        - start: The start index
        - end: The stop index (exclusive)
    - Returns the specified range of the string
- `contains(value:(string|char))->boolean`
    - Returns true if the string contains the specified value, otherwise return false
- `enumerator()->object`
    - Returns the enumerator for the current string
- `__indexer_get__[index:number]->char`
    - Returns the character at the specified index
- `getChars()->char[]`
    - Returns an array of characters that represents the string
- `getBytes()->byte[]`
    - Returns a byte array that represents the string
- `toString()->string`
    - Returns this string

### Array
An array is a fixed sized collection in which elements can be accessed through index and can store any type of value. Array is a built in reference type.
```
// Allocate a new array with initial values
items = [2, 4, 6, 8];

// Allocate an array with size 10
items = alloc [10];

// Allocate an empty array
items = [];
// or
items = alloc [0];
```
Array elements can be accessed and be set through its indexer methods `<id>[<expression>]`.
```
// Get the value stored at index 2 in the array and assign to variable
a = items[2];

// Overwrite the value
items[2] = 100;
```

An array literal is evaluated to an object that contains members.

#### Methods
- `length()->int`
    - Returns the size of the array
- `get(index:number)->any`
    - Returns the element at the specified index in the array
- `set(index:number, value:any)->void`
    - Overwrites the value at the specified index with a new value
- `enumerator()->obj`
    - Returns the enumerator for the current array
- `range(start:number, end:number)->any[]`
    - Returns a new array with the specified range of the current array
- `__indexer_get__[index:number]->any`
    - Same as get
- `__indexer_set__[index:number](value:any)->void`
    - Same as set
- `toString()->string`
    - Returns a string representation of the current array

### Dictionary
A hash based dynamically sized collection type where each entry is represented by a key value pair. Dictionary is a built in reference type.
```
// Create a dictionary with initial values
dict = {
    ["key1"] = 1,
    ["key2"] = 2,
    ["key3"] = 3,
    ["key4"] = 4,
    ["key5"] = 5
};

// Create an empty dictionary
dict = alloc { };
```

A dictionary literal is evaluated to an object that contains members.

#### Methods
- `length()->int`
    - Returns the key value pair count of the dictionary
- `get(key:any)->any`
    - Returns value for the specified key
- `set(key:any, value:any)->void`
    - Adds a key value pair or overwrites an existing value for the specified key
- `contains(key:any)->boolean`
    - Returns true if the dictionary contains the specified key, otherwise return false
- `remove(key:any)->boolean`
    - Removes the specified key if present in the dictionary. Returns true if the key was removed, otherwise return false.
- `keys()->any[]`
    - Returns an array containing the dictionary keys
- `values()->any[]`
    - Returns an array containing the dictionary values
- `enumerator()->obj`
    - Returns the enumerator for current dictionary
- `__indexer_get__[key:any]->any`
    - Same as get
- `__indexer_set__[key:any](value:any)->void`
    - Same as set
- `toString()->string`
    - Returns the string representation of this dictionary

### Object
The object data type can be used to define custom objects. Object members can be accessed through reference.
```
// Create an empty object
obj = {

};
```

```
// Create an object with initial member values
obj = {
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

```
// Access object values and assign to variables
id = obj.identifier;
col = obj.position.column;
```

```
// Add a new member to object
obj.cost = 987.01;
```