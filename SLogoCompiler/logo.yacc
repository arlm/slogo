%namespace SLogoCompiler

%start logo_program

%tokentype Token

%token TO FUNCTION_NAME END 
%token VARNAME QVARNAME NUMBER
%token OP_STAR OP_PLUS OP_MINUS OP_DIVIDE OP_OPEN OP_CLOSE
%token BLOCK_OPEN BLOCK_CLOSE
%token MAKE PU PD REPEAT
%token FD BK RT LT CS PE HOME SETPC SETSC SPS
%token HT ST
%token LABEL SETLABELHEIGHT
%token LT LE GT GE EQ AND OR NOT
%token IF ELSE WHILE STOP


/*
 * The accessibility of the Parser object must not exceed that
 * of the inherited ShiftReduceParser<,>. Thus if you want to include 
 * the *source* of ShiftReduceParser from ShiftReduceParserCode.cs, 
 * then you must either set the compilation flag EXPORT_GPPG or  
 * override the default, public visibility with %visibility internal.
 * If you reference the pre-compiled QUT.ShiftReduceParser.dll then 
 * ShiftReduceParser<> is public and either visibility will work.
 */
%visibility internal

%YYSTYPE String
%YYLTYPE LexLocation

%left '+' '-'
%left '*' '/'
%left UMINUS


%%

logo_program	:	logo_program logo_function
				|	logo_function
                ;

logo_function	:	TO FUNCTION_NAME 
					{  
						error_checker.AddFuncDef($2, "", yyLineCounter()); 
						output_code += "public void "+$2+"()\n";
						output_code += "{\n";
						output_code += "TurtleRuntime.LogoVariables<double> _lvars = new TurtleRuntime.LogoVariables<double>(_gvars);\n";
					} 
					logo_fcommands END  
					{ 
						output_code +=  $4;
						output_code +=  "\n}\n";
						
						error_checker.CloseFuncDef($2, yyLineCounter()); 
					}
				|	TO FUNCTION_NAME fparam 
					{  
						error_checker.AddFuncDef($2, $3, yyLineCounter()); 
                        output_code += "public void "+$2+"("+$3+")\n";
                        output_code += "{\n";
						output_code += "TurtleRuntime.LogoVariables<double> _lvars = new TurtleRuntime.LogoVariables<double>(_gvars);\n";
						
						string[] fps = $3.Replace("double", "").Split(',');
						foreach(string fp in fps)
						{
							output_code +=  "_lvars[\""+fp.Trim()+"\"] = "+fp+";\n";
							error_checker.AddLVarDef(fp, yyLineCounter());
						}
						
					} 
					logo_fcommands END  
					{ 
                        output_code +=  $5;
                        output_code +=  "\n}\n";
                        
                        error_checker.CloseFuncDef($2, yyLineCounter()); 

					}
				|	TO FUNCTION_NAME fparam 
					{  
						error_checker.AddFuncDef($2, $3, yyLineCounter()); 
					} 
					END  
					{ 
						output_code += "public void "+$2+"("+$3+")\n";
						output_code += "{";
						output_code +=  "\n}\n";
						
						error_checker.CloseFuncDef($2, yyLineCounter()); 
					}
				|	TO FUNCTION_NAME 
					{  
						error_checker.AddFuncDef($2, "", yyLineCounter()); 
					} 
					END  
					{ 
						output_code += "public void "+$2+"()\n";
						output_code += "{";
						output_code +=  "\n}\n";
						
						error_checker.CloseFuncDef($2, yyLineCounter()); 
					}
				;

fparam			:	VARNAME          {  $$ = "double " + $1; }
				|	fparam VARNAME   {  $$ = $1 + ", double " + $2; }
				;

logo_fcommands	:	logo_fcommands logo_command    { $$ = $1 + $2; }
				|	logo_command   { $$ = $1; }
				;


logo_bcommand	:	BLOCK_OPEN logo_fcommands BLOCK_CLOSE       { $$ = "{\n" + $2 + "\n}\n"; } 
				|	logo_command								{ $$ = "{\n" + $1 + "\n}\n"; }
				;

logo_command	: 	MAKE QVARNAME  
					{ 
						error_checker.AddLVarDef($2, yyLineCounter());
                        //$$ = "double "+$2+";\n";
                        $$ = "_lvars[\""+$2+"\"] = 0;\n";
					}
				|	MAKE QVARNAME expr
					{ 
						error_checker.AddLVarDef($2, yyLineCounter());
						//$$ = "double "+$2+" = "+$3+";\n";
                        $$ = "_lvars[\""+$2+"\"] = "+$3+";\n";
					}
				;


logo_command	:	FUNCTION_NAME					
					{ 
						error_checker.AddFuncCall($1, "", yyLineCounter());
						$$ = $1 + "();\n";			
					} 
				|	FUNCTION_NAME block_expr        
					{ 
						error_checker.AddFuncCall($1, $2, yyLineCounter());
						$$ = $1 + "(" + $2 + ");\n";	
					} 
				;

logo_command	:	LABEL QVARNAME	{	$$ = "_turtle.DrawText(\""+$2+"\");\n";	}
				;

logo_command	:	SETLABELHEIGHT expr	{ $$ = "_turtle.SetFontSize("+$2+");\n";	}
				;

logo_command	:	PU	{  $$ = "_turtle.PenUp();\n"; }
				;

logo_command	:	PD	{  $$ = "_turtle.PenDown();\n"; }
				;

logo_command	:	FD expr	{ $$ = "_turtle.Forward("+$2+");\n"; }
				;

logo_command	:	BK expr	{ $$ = "_turtle.Backward("+$2+");\n"; }
				;

logo_command	:	RT expr	{ $$ = "_turtle.Right("+$2+");\n"; }
				;

logo_command	:	LT expr	{ $$ = "_turtle.Left("+$2+");\n"; }
				;

logo_command	:	CS	{ $$ = "_turtle.Reset();\n"; }
				;

logo_command	:	PE	{ $$ = "_turtle.PenErase();\n"; }
				;

logo_command	:	HOME	{ $$ = "_turtle.Home();\n"; }
				;

logo_command	:	SETPC block_expr { $$ = "_turtle.SetPenColor("+$1+");\n"; }
				;

logo_command	:	SETSC  block_expr { $$ = "_turtle.SetPenColor("+$1+");\n"; }
				;

logo_command	:	SPS block_expr { $$ = "_turtle.SetPenSize("+$1+");\n"; }
				;

logo_command	:	STOP { $$ = "return;\n"; }
				;

logo_command	:	HT { $$ = "_turtle.Hide();\n"; }
				;

logo_command	:	ST { $$ = "_turtle.Show();\n"; }
				;


logo_command	:	REPEAT  { repeatVarCnt++; $$ = "_i"+repeatVarCnt.ToString(); } expr logo_bcommand        
					{ 
						$$ = "for(int "+$2+" = 0; "+$2+" < " + $3 +" ; "+$2+"++)\n" + $4; 
						repeatVarCnt--;
					}
                ;

logo_command	:	IF log_expr logo_bcommand   { $$ = "if ("+$2+")\n" + $3; }
				|	IF log_expr logo_bcommand  ELSE logo_bcommand { $$ = "if ("+$2+")\n" + $3 + "else\n" + $5; }
				;

logo_command	:	WHILE log_expr logo_bcommand	{ $$ = "while("+$1+")\n{\n" + $3 + "\n}\n"; }
				;

// Dodavanje vrijednosti varijabli radi se preko MAKE-a
//
//logo_command	:	VARNAME EQ expr  { $$ = $1 + " = " + $3 +";\n"; }
//				;

expr    		:   OP_OPEN expr OP_CLOSE
                {
                    $$ = "(" + $2 + ")";
                }
				|   expr OP_STAR expr
                {
                    $$ = $1 +"*"+ $3;
                }
				|   expr OP_DIVIDE expr
                {
                    $$ = $1 +"/"+ $3;
                }
				|   expr OP_PLUS expr
                {
                    $$ = $1 +"+"+ $3;
                }
				|   expr OP_MINUS expr
                {
                    $$ = $1 +"-"+ $3;
                }
				|   OP_MINUS expr %prec UMINUS
                {
                    $$ = "-" + $2;
                }
				|   VARNAME
                {
                    //$$ = $1;
                    $$ = "_lvars[\""+$1+"\"]";
                    error_checker.AddVarUsage($1, yyLineCounter());
                }
				|   NUMBER
                {
                    $$ = $1;
                }
				;



mexpr			:	mexpr  expr		{ $$ = $1 + "," + $2; }
				|	expr            { $$ = $1; }
				;


block_expr		:	BLOCK_OPEN mexpr BLOCK_CLOSE	{ $$ = $2; }
				|	mexpr                           { $$ = $1; }
				;

		
log_expr		:	OP_OPEN log_expr OP_CLOSE
                {
                    $$ = "(" + $2 + ")";
                }
				|   expr GT expr
                {
                    $$ = $1 +">"+ $3;
				}
				|   expr GE expr
                {
                    $$ = $1 +">="+ $3;
				}
				|   expr LT expr
                {
                    $$ = $1 +"<"+ $3;
				}
				|   expr LE expr
                {
                    $$ = $1 +"<="+ $3;
				}
				|   log_expr AND log_expr
                {
                    $$ = $1 +" && "+ $3;
				}
				|   log_expr OR log_expr
                {
                    $$ = $1 +" || "+ $3;
				}
				|   NOT log_expr 
                {
                    $$ = "!"+ $2;
				}
				;
			
%%

/* 
 * GPPG does not create a default parser constructor
 * Most applications will have a parser type with other
 * fields such as error handlers etc.  Here is a minimal
 * version that just adds the default scanner object.
 */


public int repeatVarCnt = 0;
public static string output_code = "";
public static string list_of_errors = "";
public static ErrorChecker error_checker = null;

private Lexer parser;

Parser(Lexer s) : base(s) 
{ 
   parser = s;
}

public long yyLineCounter()
{
     return parser.yyLineCounter();
}

public string ds()
{
     return "_debug_step(" + yyLineCounter().ToString() + ");";
}

public static string ToCs(string lgo)
{
    output_code = "TurtleRuntime.LogoVariables<double> _gvars = new TurtleRuntime.LogoVariables<double>(null);\n\n";
    error_checker = new ErrorChecker();
    list_of_errors = "";
    Parser parser = new Parser(new Lexer(GenerateStreamFromString(lgo)));
    parser.Parse();
    return output_code;
}

public static string GetErrorList()
{
	return list_of_errors;
}

private static System.IO.Stream GenerateStreamFromString(string s)
{
    System.IO.MemoryStream stream = new System.IO.MemoryStream();
    System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);
    writer.Write(s);
    writer.Flush();
    stream.Position = 0;
    return stream;
}



/*
static void Main(string[] args)
{    
    System.IO.FileStream reader;
    if (args.Length > 0)
    {
        reader = new System.IO.FileStream(args[0], System.IO.FileMode.Open);

        Parser parser = new Parser(new Lexer(reader));
        parser.Parse();
    }
    else
    {
        string code = "TO main  PD END TO main2 :pero :gjuro  PU   REPEAT 4 [ PD PU ]  PD END";

        Parser parser = new Parser(new Lexer(GenerateStreamFromString(code)));
        parser.Parse();
    }
    System.Console.WriteLine(output_code);
}

*/

class Lexer: QUT.Gppg.AbstractScanner<String, LexLocation>
{
     private Scanner scnr;

     public Lexer(System.IO.Stream reader)
     {
         scnr = new Scanner(reader);
     }

     public override int yylex()
     {
         int token = scnr.yylex(); 
         yylval = scnr.yylval;

         if(token == (int)Tokens.EOF)	// Lex EOF ? 
            token = (int)Token.EOF;     // convert to to YACC EOF.. 

         // Console.Error.WriteLine("Token --> "+token);
         return token;
     }

     public long yyLineCounter()
     {
         return scnr.yyLineCounter;
     }

     public override void yyerror(string format, params object[] args)
     {
         //list_of_errors += String.Format("Line: " + scnr.yyLineCounter + " " + format, args);
         list_of_errors += String.Format("Line " + scnr.yyLineCounter + ": Syntax error" );
     }
}


