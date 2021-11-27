grammar ZeroPoint;

root
	: (use_statement SEMICOLON)*
		statement*
	;
	
block
	: statement*
	;
	
statement
	: assign_statement SEMICOLON
	| conditional_statement
	| loop_statement
	| method_call_statement SEMICOLON
	| try_catch_statement
	| throw_statement SEMICOLON
	| indexer_set_call_statement SEMICOLON
	| compiler_const_definition
	;
	
use_statement
	: USE (IDENTIFIER|identifier_access)
	| USE IDENTIFIER IN (IDENTIFIER|identifier_access)
	;
	
assign_statement
	: IDENTIFIER assignment_operator expression
	| identifier_access assignment_operator expression
	;
	
conditional_statement
	: if_statement 
	  else_if_statement* 
	  else_statement?
	;
	
if_statement
	: IF '(' expression ')' '{'
		block
	  '}'
	;
	
else_if_statement
	: ELSE_IF '(' expression ')' '{'
		block
	  '}'
	;
	
else_statement
	: ELSE '{'
		block
	  '}'
	;
	
loop_statement
	: while_loop_statement
	| foreach_loop_statement
	;
	
while_loop_statement
	: WHILE '(' expression ')' '{'
		block
	  '}'
	;

foreach_loop_statement
	: FOREACH '(' IDENTIFIER IN expression ')' '{'
		block
	  '}'
	;
	
try_catch_statement
	: try_statement catch_statement
	;
	
try_statement
	: TRY '{' block '}'
	;
	
catch_statement
	: CATCH IDENTIFIER LAMBDA '{' block '}'
	;
	
throw_statement
	: THROW expression
	;
	
anonymous_function_definition_statement
	: function_statement
	| action_statement
	| consumer_statement
	| provider_statement
	| lambda_function_statement
	| native_function_statement
	| native_provider_statement
	;

	// Accepts parameters and returns
	function_statement
		: (
			('(' parameter_list ')') |
			IDENTIFIER
		  ) LAMBDA '{'
			block
			RETURN expression SEMICOLON
		  '}'
		;
	
	// No parameters and returns
	action_statement
		: '(' ')' LAMBDA '{'
			block
		  '}'
		;

	// Only accepts parameters
	consumer_statement
		: (
			('(' parameter_list ')') |
			IDENTIFIER
		  ) LAMBDA '{'
			block
		  '}'
		;
	
	// Only return
	provider_statement
		: '(' ')' LAMBDA '{'
			block
			RETURN expression SEMICOLON
		  '}'
		;

	//
	lambda_function_statement
		: (
			('(' ')') |
			('(' parameter_list ')') |
			IDENTIFIER
		  ) LAMBDA (expression|assign_statement)
		;
	
	// Accepts parameters, returns a value from backend referenced by inject statement
	native_function_statement
		: NATIVE '(' parameter_list ')' LAMBDA inject_statement
		;
	
	// Only return, same as above
	native_provider_statement
		: NATIVE '(' ')' LAMBDA inject_statement
		;


	// ++Special Methods++
	operator_function_statement
		: binary_operator_function_statement
		| unary_operator_function_statement
		;

		binary_operator_function_statement
			: OPERATOR IDENTIFIER '[' 
				op=(MULT|DIV|MOD|PLUS|MINUS|SHIFT_LEFT|SHIFT_RIGHT|LESS_THAN|LESS_THAN_OR_EQUAL|GREATER_THAN|GREATER_THAN_OR_EQUAL|EQUAL|NOTEQUAL|BITWISE_AND|BITWISE_XOR|BITWISE_OR|AND|OR) 
			  ']' IDENTIFIER LAMBDA (('{' block RETURN expression SEMICOLON '}') | expression)
			;

		unary_operator_function_statement
			: OPERATOR UNARY '[' op=(EXCLAMATION_MARK|MINUS) ']' IDENTIFIER LAMBDA (('{' block RETURN expression SEMICOLON '}') | expression)
			;

	indexer_method_statement
		: indexer_get_statement
		| indexer_set_statement
		;
			
			// Syntax: indexer [a1, a2, a3, ...] => { block return expression; }
			//									  => expression
			indexer_get_statement
				: INDEXER '[' parameter_list ']' LAMBDA (('{' block RETURN expression SEMICOLON '}') | expression)
				;

			// Syntax: indexer [a1, a2, a3, ...]<-b => { block; }
			//										 => expression
			indexer_set_statement
				: INDEXER '[' parameter_list ']' LEFT_POINT_ARROW IDENTIFIER LAMBDA (('{' block '}') | expression)
				;

	// --Special Methods--


inject_statement
	: '<' '@' STRING '>'
	;

compiler_const_definition
	: CTCONST IDENTIFIER expression
	;
	
method_call_statement
	: paren_call
	| indexer_get_call_statement
	;

	paren_call
		: (IDENTIFIER|identifier_access) '(' argument_list? ')'
		;

	indexer_get_call_statement // expression (return value)
		: (IDENTIFIER|identifier_access) '[' argument_list ']'
		;

indexer_set_call_statement // statement (no return value)
	: (IDENTIFIER|identifier_access) '[' argument_list ']' ASSIGN expression
	;
	
object_initialization_expression
	: '{'
			(assign_statement | operator_function_statement | indexer_method_statement)
			(',' (assign_statement | operator_function_statement | indexer_method_statement))*
	  '}'
	| '{' '}'
	;

expression
	: '(' expression ')'																		
	| <assoc=right> op=(EXCLAMATION_MARK|MINUS) expression										
	| expression op=(MULT|DIV|MOD) expression													
	| expression op=(PLUS|MINUS) expression														
	| expression op=(SHIFT_LEFT|SHIFT_RIGHT) expression											
	| expression op=(LESS_THAN|LESS_THAN_OR_EQUAL|GREATER_THAN|GREATER_THAN_OR_EQUAL) expression
	| expression op=(EQUAL|NOTEQUAL|STRICT_EQUAL|STRICT_NOTEQUAL) expression					
	| expression op=BITWISE_AND expression														
	| expression op=BITWISE_XOR expression														
	| expression op=BITWISE_OR expression														
	| expression op=AND expression																
	| expression op=OR expression																
	| <assoc=right> expression QUESTION_MARK expression COLON expression
	| atom
	;
	
identifier_access
	: IDENTIFIER ACCESSOR IDENTIFIER (ACCESSOR IDENTIFIER)*
	| ACCESSOR IDENTIFIER (ACCESSOR IDENTIFIER)*
	;
	

atom
	: literal								
	| IDENTIFIER							
	| identifier_access						
	| method_call_statement				
	| object_initialization_expression		
	| anonymous_function_definition_statement
	| array_literal_notation
	| dictionary_literal_notation
	;

literal
	: lit=STRING
	| lit=CHAR
	| lit=BOOLEAN
	| lit=NUMBER
	| lit=NULL
	| lit=VOID
	;

array_literal_notation
	: '[' argument_list ']'
	| '[' ']'
	| ALLOC '[' argument_list ']'
	;

dictionary_literal_notation
	: '{' dictionary_arguments '}'
	| ALLOC '{' '}'
	;
	
	dictionary_arguments
		: '[' dictionary_key ']' ASSIGN dictionary_value
			(',' '[' dictionary_key ']' ASSIGN dictionary_value)*
		;
		
		dictionary_key: expression;

		dictionary_value: expression;
	
assignment_operator
	: op=ASSIGN
	| op=ADD_ASSIGN
	| op=SUB_ASSIGN
	| op=MULT_ASSIGN
	| op=DIV_ASSIGN
	| op=MOD_ASSIGN
	| op=AND_ASSIGN
	| op=XOR_ASSIGN
	| op=OR_ASSIGN
	| op=BITWISE_AND_ASSIGN
	| op=BITWISE_XOR_ASSIGN
	| op=BITWISE_OR_ASSIGN
	| op=SHIFT_LEFT_ASSIGN
	| op=SHIFT_RIGHT_ASSIGN
	;
	
parameter_list
	: IDENTIFIER (',' IDENTIFIER)*
	;
	
argument_list
	: expression (',' expression)*
	;
	
WS
	: [ \t\r\n]+ -> skip
	;
	
LINE_COMMENT
    : '//' ~[\r\n]* -> skip
	;

// ++ Binary Operators ++
PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';

EQUAL: '==';
STRICT_EQUAL: '===';
NOTEQUAL: '!=';
STRICT_NOTEQUAL: '!==';
LESS_THAN: '<';
LESS_THAN_OR_EQUAL: '<=';
GREATER_THAN: '>';
GREATER_THAN_OR_EQUAL: '>=';

AND: '&&';
XOR: '^^';
OR: '||';
BITWISE_AND: '&';
BITWISE_XOR: '^';
BITWISE_OR: '|';

SHIFT_LEFT: '<<';
SHIFT_RIGHT: '>>';
// -- Binary Operators --

// ++ Assignment Operators (binary operators) ++
ASSIGN: '=';
ADD_ASSIGN: '+=';
SUB_ASSIGN: '-=';
MULT_ASSIGN: '*=';
DIV_ASSIGN: '/=';
MOD_ASSIGN: '%=';
AND_ASSIGN: '&&=';
XOR_ASSIGN: '^^=';
OR_ASSIGN: '||=';
BITWISE_AND_ASSIGN: '&=';
BITWISE_XOR_ASSIGN: '^=';
BITWISE_OR_ASSIGN: '|=';
SHIFT_LEFT_ASSIGN: '<<=';
SHIFT_RIGHT_ASSIGN: '>>=';
// -- Assignment Operators --
	
ACCESSOR: '.';
	
IF: 'if';
ELSE_IF: 'else' [ \t\r\n]+ 'if';
ELSE: 'else';

BOOLEAN
	: 'true'
	| 'false'
	;
	
NUMBER
	: DECIMAL_NUMBER
	| INTEGER_NUMBER
	| BYTE_NUMBER
	;

	// The AST mapper decides if the values are in range!
	BYTE_NUMBER
		: 'b\'' INTEGER_NUMBER
		| 'b\'' BINARY
		| 'b\'' HEX
		;

	INTEGER_NUMBER
		: [0-9]+
		| BINARY
		| HEX
		;

	DECIMAL_NUMBER
		: [0-9]+ '.' [0-9]+
		| '.' [0-9]+
		;

	// Used internally by NUMBER
	BINARY
		: '0b' [01]+
		;

	HEX
		: '0x' ([0-9]|[a-fA-F])+
		;
	
STRING
	: '"' (~('"' | '\\' | '\n') | '\\' ('"' | '\\' | 'n' | 't'))+ '"'
	| '""'
	;

CHAR
	: '\'' (~('\'' | '\\' | '\n') | '\\' ('\'' | '\\' | 'n' | 't' | 'x'))+ '\''
	| '\'\''
	;
	
NULL
	: 'null'
	;

VOID
	: 'void'
	;
	
USE: 'use';
IN: 'in';
NATIVE: 'native';
OPERATOR: 'operator';
UNARY: 'unary';
INDEXER: 'indexer';
ALLOC: 'alloc';

WHILE: 'while';
FOREACH: 'foreach';

RETURN: 'return';

TRY: 'try';
CATCH: 'catch';
THROW: 'throw';

CTCONST: '#const';

IDENTIFIER
	: [A-Za-z0-9_]+
	;

LAMBDA: '=>';
LEFT_POINT_ARROW: '<-';
QUESTION_MARK: '?';
COLON: ':';
EXCLAMATION_MARK: '!';
SEMICOLON: ';';
