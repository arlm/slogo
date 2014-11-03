%namespace SLogoCompiler

%option verbose, caseInsensitive, noparser, summary

alpha          [a-zA-Z]
numeric        [0-9]
alphanumeric   [a-zA-Z0-9_]

%%

PU|PenUp				{ return (int)Token.PU;    } 
PD|PenDown				{ return (int)Token.PD;    } 
REPEAT					{ return (int)Token.REPEAT;} 
MAKE					{ return (int)Token.MAKE;  } 
FD|Forward				{ return (int)Token.FD;    } 
BK|Back					{ return (int)Token.BK;    } 
RT|Right				{ return (int)Token.RT;    } 
LT|Left					{ return (int)Token.LT;    } 
CS|ClearScreen			{ return (int)Token.CS;    } 
PE|PenErase				{ return (int)Token.PE;    } 
Home					{ return (int)Token.HOME;  } 
SETPC					{ return (int)Token.SETPC; } 
SETSC					{ return (int)Token.SETSC; } 
SetPenSize				{ return (int)Token.SPS;   } 
Label					{ return (int)Token.LABEL; } 
SetLabelHeight			{ return (int)Token.SETLABELHEIGHT; } 
HT|HideTurtle			{ return (int)Token.HT; } 
ST|ShowTurtle			{ return (int)Token.ST; } 

Stop					{ return (int)Token.STOP;  } 

TO						{ return (int)Token.TO;    } 
END						{ return (int)Token.END;   } 

AND							{ return (int)Token.AND; }
OR							{ return (int)Token.OR; }
NOT							{ return (int)Token.NOT; }
IF							{ return (int)Token.IF; }
ELSE						{ return (int)Token.ELSE; }
WHILE						{ return (int)Token.WHILE; }

\"{alpha}{alphanumeric}*	{  yylval = yytext.ToLower().Substring(1); return (int)Token.QVARNAME;	}
\:{alpha}{alphanumeric}*	{  yylval = yytext.ToLower().Substring(1); return (int)Token.VARNAME;	}
{alpha}{alphanumeric}*    	{  yylval = yytext.ToLower(); return (int)Token.FUNCTION_NAME; } 
__code_2_run    			{  yylval = yytext.ToLower(); return (int)Token.FUNCTION_NAME; } 

#B[0|1]*					{  yylval = yytext; /*System.Console.WriteLine("Binary: " + yytext);*/ return (int)Token.NUMBER; }
#H[0-9A-F]* 				{  yylval = "0x"+yytext; /*System.Console.WriteLine("Hex: " + yytext);*/ return (int)Token.NUMBER; }
#O[0-7]*					{  yylval = "0"+yytext; /*System.Console.WriteLine("Octal: " + yytext);*/ return (int)Token.NUMBER; }
#D[0-9]*					{  yylval = yytext.Substring(2); /*System.Console.WriteLine("Decimal: " + yytext);*/ return (int)Token.NUMBER; }
[0-9]*[\.]*[0-9]*			{  yylval = yytext; /*System.Console.WriteLine("Decimal: " + yytext);*/ return (int)Token.NUMBER; }


\* 							{ return (int)Token.OP_STAR ; }
\+ 							{ return (int)Token.OP_PLUS ; }
\- 							{ return (int)Token.OP_MINUS ; }
\/ 							{ return (int)Token.OP_DIVIDE; }
\( 							{ return (int)Token.OP_OPEN; }
\) 							{ return (int)Token.OP_CLOSE; }
\[ 							{ return (int)Token.BLOCK_OPEN; }
\] 							{ return (int)Token.BLOCK_CLOSE; }

\<\=						{ return (int)Token.LE; }
\>\=						{ return (int)Token.GE; }
\>							{ return (int)Token.GT; }
\<							{ return (int)Token.LT; }
\=							{ return (int)Token.EQ; }


\n|\r\n?					yyLineCounter++;    // Ignore newlines ..
[ \t]*						;					// Ignore spaces and tabs..
.							;					// Ignore rest of characters.. 

%%

public String yylval;	// Polje koje vraca string vrijednost (brojevi, imena varijabli, itd...) gramatici !!!!
public long yyLineCounter = 1;	// Brojac linija
