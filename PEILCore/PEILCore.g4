grammar PEILCore;

program
 : (singleid|del|retu|assign|addSubAssign|runsinglefunc|ifStatement|whileStatement)* #Lines
 ;

functions : (deffunc)* ;
classes : (defineclass)* ;
importlibrary : (importlib)* ;
runsinglefunc : (inclass)* runFunc EOL ;
del : 'del' (id ',')* id ';' ;
singleid : id ';' ;

expr
 : (inclass)* runFunc #RunFunction
 | expr oper = ('=='|'!='|'>='|'<='|'<'|'>') expr #Compare
 | '(' expr ')' #MoreExpr
 | expr oper = (MUL | DIV) expr #MulDiv
 | expr oper = (ADD | SUB) expr	#AddSub
 | (inclass)* id #Ident
 | NUM	#Number
 | STRING #Str
 ;
id : ID ;
assign : (inclass)* id '=' expr EOL ;
addSubAssign : (inclass)* id oper = ('+=' | '-=' ) expr EOL ;
runFunc : ID params ;
params : '(' vars ')' ;
vars : (expr (',' expr)*)* ;
ifStatement : 'if' '(' expr ')' '{' program '}' ('else' '{' program '}')? ;
whileStatement : 'while' '(' expr ')' '{' program '}' ;
deffunc : 'func' ID params '{' program '}' #DefFunction ;
retu : 'return' expr EOL #Return ;
defineclass : 'class' ID ( 'ext' ID )? '{' (defineclass|deffunc|assign)* '}';
classmember : ID | runFunc ;
inclass
 : id '.' #ID_inClass
 | runFunc '.' #Func_inClass
 ;
importlib : 'import' ID EOL ;

ADD : '+' ;
SUB : '-' ;
MUL : '*' ;
DIV : '/' ;
ID : [a-z_A-Z]+ [0-9a-z_A-Z]* ;
NUM : '-'? ('.' [0-9]+ | [0-9]+ ('.' [0-9]*)? ) ;
STRING : '"' ( ~( '"' ) | '\\"' )* '"' ;
WS : ( ' ' | '\t' | '\n' | '\r' )+ {Skip();} ;
EXPLAIN : ('/*' .* '*/' | '//' .* ('\t' | '\n' | '\r') ) {Skip();} ;
EOL : ';' ;