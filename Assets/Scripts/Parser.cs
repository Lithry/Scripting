using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parser 
{
	enum State
	{
		Ident,
		Arguments
	}

	List<Instruction> instructions = new List<Instruction>();

	Tokenizer tokenizer = new Tokenizer();

	State state = State.Ident;

	Tables tables;

	public Parser(Tables tables)
	{
		this.tables = tables;
	}

	public void Reset()
	{
		tables.Clear();
		state = State.Ident;
		instructions.Clear();
	}

	public bool Parse(string str, out List<Instruction> instructions)
	{
		Reset();
		tokenizer.Start(str);

		Instruction currentInstruction = new Instruction();

		instructions = this.instructions;

		Tokenizer.Token currentToken = tokenizer.GetNextToken();

		if (currentToken.Type == Tokenizer.TokenType.Empty)
		{
			return false;
		}
		
		while (currentToken.Type != Tokenizer.TokenType.EOF && currentToken.Type != Tokenizer.TokenType.Unknown)
		{
			switch(state)
			{
				case State.Ident:
					if (currentToken.Type != Tokenizer.TokenType.Ident)
						return false;

					string ident = currentToken.Lexeme;

					currentToken = tokenizer.GetNextToken(); // skip to next token

					if (currentToken.Type == Tokenizer.TokenType.Colon) // If it's a colon, then the ident is a Label
					{
						if (!tables.AddLabel(ident, instructions.Count)) // Couldn't add label, probably already exists
							return false;
						
						currentInstruction = new Instruction(); 
						currentInstruction.OpCode = OpCodes.NOP;
						instructions.Add(currentInstruction); // We add a NOP instruction just to be sure 

						currentToken = tokenizer.GetNextToken(); // skip to next token

						if (currentToken.Type != Tokenizer.TokenType.EOL && currentToken.Type != Tokenizer.TokenType.EOF)
						{
							return false;
						}
					}
					else // If it isn't, then probably it's an instruction
					{
						int opCode = 0;

						if (!tables.GetInstrLookUp(ident, out opCode))
							return false;

						currentInstruction = new Instruction();

						currentInstruction.OpCode = opCode;

						if (currentToken.Type == Tokenizer.TokenType.OpenParent)
						{
							state = State.Arguments;
							currentToken = tokenizer.GetNextToken(); // Skip parenthesis

							currentInstruction.Arguments = new List<string>();
						}
						else if (currentToken.Type != Tokenizer.TokenType.EOL && currentToken.Type != Tokenizer.TokenType.EOF)
							return false;
					}

				break;

				case State.Arguments:
					if (currentToken.Type == Tokenizer.TokenType.Number || currentToken.Type == Tokenizer.TokenType.String)
					{
						currentInstruction.Arguments.Add(currentToken.Lexeme);

						currentToken = tokenizer.GetNextToken();
						
						if (currentToken.Type == Tokenizer.TokenType.Comma)
							currentToken = tokenizer.GetNextToken(); // skip comma
						else if (currentToken.Type == Tokenizer.TokenType.CloseParent)
						{
							currentToken = tokenizer.GetNextToken();

							if (currentToken.Type != Tokenizer.TokenType.EOL && currentToken.Type != Tokenizer.TokenType.EOF)
								return false;
							
							instructions.Add(currentInstruction);

							state = State.Ident;
						}
						else
							return false; // Syntax error! 
					}
					else if (currentToken.Type == Tokenizer.TokenType.Ident) // If there's an identifier, then maybe is a GoTo
					{
						Label label;
						
						if (!tables.GetLabelByName(currentToken.Lexeme, out label))
							return false;

						currentInstruction.Arguments.Add(label.Index.ToString());

						currentToken = tokenizer.GetNextToken();
					}
					else if (currentToken.Type == Tokenizer.TokenType.CloseParent)
					{
						currentToken = tokenizer.GetNextToken();

						if (currentToken.Type != Tokenizer.TokenType.EOL && currentToken.Type != Tokenizer.TokenType.EOF)
							return false; // Syntax error! 

						instructions.Add(currentInstruction);

						state = State.Ident;
					}
					else
					{
						return false; // Syntax error!
					}
				break;
			}

			SkipEOL(); // Skips End of Lines
			
			currentToken = tokenizer.GetCurrentToken();
		}

		return true;
	}

	void SkipEOL()
	{
		while (tokenizer.GetCurrentToken().Type == Tokenizer.TokenType.EOL)
			tokenizer.GetNextToken();
	}
}
