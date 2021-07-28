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
	| function_call_statement SEMICOLON
	| try_catch_statement
	| throw_statement SEMICOLON
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
	;
	
while_loop_statement
	: WHILE '(' expression ')' '{'
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


inject_statement
	: '<' '@' STRING '>'
	;
	
function_call_statement
	: (IDENTIFIER|identifier_access) '(' argument_list? ')'
	;
	
object_initialization_expression
	: '{'
			assign_statement
			(',' assign_statement)*
	  '}'
	| '{' '}'
	;

expression
	: '(' expression ')'
	| expression (MULT|DIV|MOD) expression
	| expression (PLUS|MINUS) expression
	| expression (SHIFT_LEFT|SHIFT_RIGHT) expression
	| expression (LESS_THAN|LESS_THAN_OR_EQUAL|GREATER_THAN|GREATER_THAN_OR_EQUAL) expression
	| expression (EQUAL|NOTEQUAL) expression
	| expression BITWISE_AND expression
	| expression BITWISE_XOR expression
	| expression BITWISE_OR expression
	| expression AND expression
	| expression OR expression
	| <assoc=right> expression QUESTION_MARK expression COLON expression	// Conditional ternary expression
	| literal
	| IDENTIFIER
	| identifier_access
	| function_call_statement
	| object_initialization_expression
	| anonymous_function_definition_statement
	;
	
identifier_access
	: IDENTIFIER ACCESSOR IDENTIFIER (ACCESSOR IDENTIFIER)*
	| ACCESSOR IDENTIFIER (ACCESSOR IDENTIFIER)*
	;
	
literal
	: STRING
	| BOOLEAN
	| NUMBER
	| NULL
	;
	
assignment_operator
	: ASSIGN
	| ADD_ASSIGN
	| SUB_ASSIGN
	| MULT_ASSIGN
	| DIV_ASSIGN
	| MOD_ASSIGN
	| AND_ASSIGN
	| XOR_ASSIGN
	| OR_ASSIGN
	| BITWISE_AND_ASSIGN
	| BITWISE_XOR_ASSIGN
	| BITWISE_OR_ASSIGN
	| SHIFT_LEFT_ASSIGN
	| SHIFT_RIGHT_ASSIGN
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
	
PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';

EQUAL: '==';
NOTEQUAL: '!=';
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
	;

	INTEGER_NUMBER
		: [0-9]+
		;

	DECIMAL_NUMBER
		: [0-9]+ '.' [0-9]+
		| '.' [0-9]+
		;
	
STRING
	: '"' (~('"' | '\\' | '\n') | '\\' ('"' | '\\' | 'n' | 't'))+ '"'
	| '""'
	;
	
NULL
	: 'null'
	;
	
USE: 'use';
IN: 'in';
NATIVE: 'native';

WHILE: 'while';

RETURN: 'return';

TRY: 'try';
CATCH: 'catch';
THROW: 'throw';

IDENTIFIER
	: [A-Za-z0-9_]+
	;

LAMBDA: '=>';
QUESTION_MARK: '?';
COLON: ':';
SEMICOLON: ';';
