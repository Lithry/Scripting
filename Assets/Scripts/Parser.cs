using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	; COMMENT

	FUNC PEPE 

		VAR W

		POP C
		POP B
		POP A


		RET
	ENDFUNC

	LABEL:
		VAR X

		MOV X, 5

		PUSH A
		PUSH B
		PUSH C
		JSR PEPE

		JMP LABEL

 */

public class Parser 
{
	Tokenizer tokenizer = new Tokenizer();

	Tables tables;
	ErrorManager errorHandler;
	int scope = -1;

	public Parser(Tables tables, ErrorManager errorHandler)
	{
		this.tables = tables;
		this.errorHandler = errorHandler;
	}

	public void Reset()
	{
		tables.Clear();
		scope = -1;
	}

	public bool Parse(string str)
	{
		Reset();

		tokenizer.Start(str);

		// Parse Vars and Labels
		if (!Pass1())
			return false;

		// Rewind tokenizer to start the
		// second pass from the beginning
		tokenizer.Rewind();

		// Parse instructions
		if (!Pass2())
			return false;

		return true;
	}

	// Parse Vars and Labels
	bool Pass1()
	{
		Tokenizer.Token currentToken = tokenizer.GetNextToken();

		if (currentToken.Type == Tokenizer.TokenType.Empty)
		{
			return false;
		}

		int instrIdx = 0;
		
		while (currentToken.Type != Tokenizer.TokenType.EOF && currentToken.Type != Tokenizer.TokenType.Unknown)
		{
			// ===================================================================
			// Skip end of lines
			if (currentToken.Type == Tokenizer.TokenType.EOL)
			{
				currentToken = tokenizer.GetNextToken();
			}
			// ===================================================================
			// Parse variables
 			else if (currentToken.Type == Tokenizer.TokenType.Rsvd_Var)
			{
				currentToken = tokenizer.GetNextToken();
				
				if (currentToken.Type == Tokenizer.TokenType.Ident)
				{
					if (!tables.AddVar(currentToken.Lexeme, scope))
					{
						errorHandler.ParserLogError("Var Already Exists");
						return false;
					}

					if (scope != -1){
						if (!tables.FuncIncrementFrameSize(scope)){
							errorHandler.ParserLogError("Could not Find Function that Corresponds to Scope");
							return false;
						}
					}
				}
				else
				{
					errorHandler.ParserLogError("Ident Expected");
					return false;
				}

				currentToken = tokenizer.GetNextToken();
			}
			// ===================================================================
			// Parse functions
			else if (currentToken.Type == Tokenizer.TokenType.Rsvd_Func)
			{
				if (scope != -1)
				{
					errorHandler.ParserLogError("Declaring Functions in Functions is Illegal");
					return false;
				}
				
				currentToken = tokenizer.GetNextToken();
				
				if (currentToken.Type == Tokenizer.TokenType.Ident)
				{
					if (!tables.AddFunc(currentToken.Lexeme, instrIdx, out scope))
					{
						errorHandler.ParserLogError("Function Already Exists");
						return false;
					}
				}
				else
				{
					errorHandler.ParserLogError("Ident Expected");
					return false;
				}

				currentToken = tokenizer.GetNextToken();
			}
			else if (currentToken.Type == Tokenizer.TokenType.Rsvd_EndFunc)
			{
				scope = -1;
				currentToken = tokenizer.GetNextToken();
			}
			// ===================================================================
			// Parse instructions and labels			
			else if (currentToken.Type == Tokenizer.TokenType.Ident)
			{
				string ident = currentToken.Lexeme;

				currentToken = tokenizer.GetNextToken();

				// ===================================================================
				// Is it a label?
				if (currentToken.Type == Tokenizer.TokenType.Colon)
				{
					tables.AddLabel(ident, instrIdx, scope);

					currentToken = tokenizer.GetNextToken();
				}
				// ===================================================================
				// It's an instruction
				else
				{
					if (scope == -1 && tables.GetStartPC() == -1)
						tables.SetStartPC(instrIdx);

					instrIdx++; // Increment counter

					// Skip to next line
					if (currentToken.Type != Tokenizer.TokenType.EOL)
						currentToken = tokenizer.SkipToNextLine();
				}

			}
			else
			{
				errorHandler.ParserLogError("Unexpected Token");
				return false;
			}
		}

		return true;
	}

	// Parse instructions
	bool Pass2()
	{
		scope = -1;
		Instruction currentInstruction;
		Tokenizer.Token currentToken = tokenizer.GetNextToken();

		if (currentToken.Type == Tokenizer.TokenType.Empty)
		{
			return false;
		}
		
		while (currentToken.Type != Tokenizer.TokenType.EOF && currentToken.Type != Tokenizer.TokenType.Unknown)
		{
			// ===================================================================
			// Skip end of lines
			if (currentToken.Type == Tokenizer.TokenType.EOL)
			{
				currentToken = tokenizer.GetNextToken();
			}
			// ===================================================================
			// Skip variables declaration 
 			else if (currentToken.Type == Tokenizer.TokenType.Rsvd_Var)
			{
				currentToken = tokenizer.GetNextToken(); // Skip the VAR reserved word
				
				currentToken = tokenizer.GetNextToken(); // Skip VAR's identifier
			}
			else if (currentToken.Type == Tokenizer.TokenType.Rsvd_Func)
			{
				currentToken = tokenizer.GetNextToken(); // Skip the FUNC reserved word

				FuncDecl func;
				if (!tables.GetFuncByIdent(currentToken.Lexeme, out func)){
					// Error imposible
					return false;
				}
				scope = func.scope;

				currentToken = tokenizer.GetNextToken(); // Skip FUNC`s identifier
			}
			else if (currentToken.Type == Tokenizer.TokenType.Rsvd_EndFunc)
			{
				scope = -1;
				currentToken = tokenizer.GetNextToken(); // Skip the ENDFUNC reserved word
			}
			// ===================================================================
			// Parse instructions and labels			
			else if (currentToken.Type == Tokenizer.TokenType.Ident)
			{
				string ident = currentToken.Lexeme;

				currentToken = tokenizer.GetNextToken();

				// ===================================================================
				// Is it a label? Skip it
				if (currentToken.Type == Tokenizer.TokenType.Colon)
				{
					currentToken = tokenizer.GetNextToken();
				}
				// ===================================================================
				// It's an instruction
				else
				{
					InstrDecl instr;

					if (!tables.GetInstrLookUp(ident, out instr))
					{
						errorHandler.ParserLogError("Syntax Error");
						return false;
					}

					currentInstruction = new Instruction();
					currentInstruction.OpCode = instr.OpCode;

					if (instr.ParamsCount > 0)
						currentInstruction.Values = new Value[instr.ParamsCount];

					// ===================================================================
					// Parse params
					for (int i = 0; i < instr.ParamsCount; i++)
					{
						// We have to skip the ','
						if (i > 0)
						{
							currentToken = tokenizer.GetNextToken();
							if (currentToken.Type != Tokenizer.TokenType.Comma)
							{
								errorHandler.ParserLogError("Comma Expected");
								return false;
							}
							
							currentToken = tokenizer.GetNextToken();
						}

						Tokenizer.TokenType t = currentToken.Type;
						int flags = instr.ParamsFlags[i];

						// ===================================================================
						// Is it a variable or label?
						if (t == Tokenizer.TokenType.Ident)
						{
							if ((flags & OpFlags.MemIdx) != 0)
							{
								VarDecl varDecl;
								
								if (!tables.GetVarByIdent(currentToken.Lexeme, out varDecl, scope))
								{
									errorHandler.ParserLogError("Variable Doesn´t Exist");
									return false;
								}
								if (varDecl.scope == -1)
									currentInstruction.Values[i].Type = OpType.AbsMemIdx;
								else
									currentInstruction.Values[i].Type = OpType.RelMemIdx;

								currentInstruction.Values[i].StackIndex = varDecl.Idx;
							}
							else if ((flags & OpFlags.InstrIdx) != 0)
							{
								LabelDecl label;
								
								if (!tables.GetLabelByName(currentToken.Lexeme, out label, scope))
								{
									errorHandler.ParserLogError("Label Doesn´t Exist");
									return false;
								}

								currentInstruction.Values[i].Type = OpType.InstrIdx;
								currentInstruction.Values[i].InstrIndex = label.Idx;
							}
							else if ((flags & OpFlags.HostAPICallIdx) != 0)
							{
								currentInstruction.Values[i].Type = OpType.HostAPICallString;
								currentInstruction.Values[i].StringLiteral = currentToken.Lexeme;
								// TODO: host api calls
							}
						}
						// ===================================================================
						// Is it a literal value?
						else if (t == Tokenizer.TokenType.Number || t == Tokenizer.TokenType.String)
						{
							if ((flags & OpFlags.Literal) == 0)
							{	
								errorHandler.ParserLogError("Doesn´t Allow Literals");
								return false;
							}

							if (t == Tokenizer.TokenType.Number)
							{
								if (StringUtil.IsStringFloat(currentToken.Lexeme))
								{
									float val = 0;
									
									currentInstruction.Values[i].Type = OpType.Float;

									if (float.TryParse(currentToken.Lexeme, out val))
										currentInstruction.Values[i].FloatLiteral = val;
									else
									{
										errorHandler.ParserLogError("Error Parsing Float Value");
										return false;
									}
								}
								else if (StringUtil.IsStringInt(currentToken.Lexeme))
								{
									int val = 0;
									
									currentInstruction.Values[i].Type = OpType.Int;

									if (int.TryParse(currentToken.Lexeme, out val))
										currentInstruction.Values[i].IntLiteral = val;
									else
									{
										errorHandler.ParserLogError("Error Parsing Int Value");
										return false;
									}
								}
								else if (StringUtil.IsStringHex(currentToken.Lexeme))
								{
									currentInstruction.Values[i].Type = OpType.Int;
									currentInstruction.Values[i].IntLiteral = StringUtil.StrHexToInt(currentToken.Lexeme);
								}
								else 
								{
									errorHandler.ParserLogError("Error Parsing Literal Value");
									return false;
								}
							}
							else
							{
								currentInstruction.Values[i].Type = OpType.String;
								currentInstruction.Values[i].StringLiteral = currentToken.Lexeme;
							}
						}
						else
						{
							errorHandler.ParserLogError("Unexpected Token");
							return false;
						}
					}

					// Add the instruction to the stream
					tables.AddInstrToStream(currentInstruction);
					
					// Skip to next token
					currentToken = tokenizer.GetNextToken();
				}
			}
			else
			{
				errorHandler.ParserLogError("Unexpected Token");
				return false;
			}
		}

		return true;
	}

}
