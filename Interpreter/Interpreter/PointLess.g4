grammar PointLess;

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
	;
	
use_statement
	: USE (IDENTIFIER|identifier_access)
	| USE IDENTIFIER IN (IDENTIFIER|identifier_access)
	;
	
assign_statement
	: IDENTIFIER ASSIGN expression
	| identifier_access ASSIGN expression
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
	: for_loop_statement
	| while_loop_statement
	;
	
for_loop_statement
	: FOR '(' for_loop_initialization ')' '{'
		block
	  '}'
	;
	
for_loop_initialization
	: assign_statement IN expression '..' expression
	| IDENTIFIER IN IDENTIFIER
	| IDENTIFIER IN identifier_access
	;
	
while_loop_statement
	: WHILE '(' expression ')' '{'
		block
	  '}'
	;
	
// Functions
anonymous_function_definition_statement
	: function_statement
	| action_statement
	| consumer_statement
	| provider_statement
	| native_function_statement
	| native_provider_statement
	;

function_statement
	: '(' parameter_list ')' LAMBDA '{'
		block
		RETURN expression SEMICOLON
	  '}'
	;
	
action_statement
	: '(' ')' LAMBDA '{'
		block
	  '}'
	;

consumer_statement
	: '(' parameter_list ')' LAMBDA '{'
		block
	  '}'
	;
	
provider_statement
	: '(' ')' LAMBDA '{'
		block
		RETURN expression SEMICOLON
	  '}'
	;
	
native_function_statement
	: NATIVE '(' parameter_list ')' LAMBDA inject_statement
	;
	
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

// Unary
INCREMENT: '++';
DECREMENT: '--';
EXCLAMATION: '!';
	
ACCESSOR: '.';
	
IF: 'if';
ELSE_IF: 'else' [ \t\r\n]+ 'if';
ELSE: 'else';

BOOLEAN
	: 'true'
	| 'false'
	;
	
NUMBER
	: [0-9]+
	;
	
STRING
	: '"' ~('"')+ '"'
	| '""'
	;
	
NULL
	: 'null'
	;
	
USE: 'use';
IN: 'in';
NATIVE: 'native';

FOR: 'for';
WHILE: 'while';

RETURN: 'return';

IDENTIFIER
	: [A-Za-z0-9_]+
	;

LAMBDA: '=>';
SEMICOLON: ';';
