grammar Exprs;

expressions: Nl* (expression Nl+)* expression Nl* EOF;

expression: firstExpr expr* lastExpr '=' Number;

firstExpr: Neg? Number? Token;
expr: (Neg | Plus) Number? Token;
lastExpr: Plus Number;

Neg: '-';
Plus: '+';

Token: [A-Za-z]+;
Number: [0-9]+;

Nl: [\n\r]+;

WS: [ \t] -> skip;