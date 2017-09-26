using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tokenizer  
{
    public enum TokenType
    {
        Ident, 
        OpenParent,
        CloseParent, 
        Colon, // Dos puntos
        Comma,
        String,
        Number,
        EOL, // Fin de linea
        EOF, // Fin de archivo
        Unknown,
        Empty
    }

    public struct Token
    {
        public string Lexeme;
        public TokenType Type;
    }

    public enum State
    {
        None,
        String
    }
    private State currentState = State.None;

    private string currentString; 
    private int idStart = 0;
    private int idEnd = 0;
    private Token token = new Token();

    public void Start(string str)
    {
        Reset();
        currentString = RemoveSpaces(str); // Changed!
    }

    public void Reset()
    {
        currentString = null;
        idStart = 0;
        idEnd = 0;
    }

    public Token GetCurrentToken()
    {
        return token;
    }
    
    public Token GetNextToken()
    {
        idStart = idEnd;

        if (string.IsNullOrEmpty(currentString))
        {
            token.Lexeme = "";
            token.Type = TokenType.Empty;
        }
        else if (idStart >= currentString.Length)
        {
            token.Lexeme = "";
            token.Type = TokenType.EOF;
        }
        else
        {
            if (char.IsLetter(currentString[idStart]))
            {
                token.Type = TokenType.Ident;
                token.Lexeme = GetLexemeFromString(currentString);
            }
            else if (char.IsDigit(currentString[idStart]))
            {
                token.Type = TokenType.Number;
                token.Lexeme = GetLexemeFromString(currentString);
            }
            else if (IsCarriageReturn(currentString[idStart]))
            {
                token.Lexeme = currentString[idStart].ToString();
                token.Type = TokenType.EOL;
                idEnd++;
            }
            else if (currentString[idStart] == ':') // Agregamos el : para numeros de linea
            {
                token.Lexeme = currentString[idStart].ToString();
                token.Type = TokenType.Colon;
                idEnd++;
            }
            else if (currentString[idStart] == '(')
            {
                token.Lexeme = currentString[idStart].ToString();
                token.Type = TokenType.OpenParent;
                idEnd++;
            }
            else if (currentString[idStart] == ')')
            {
                token.Lexeme = currentString[idStart].ToString();
                token.Type = TokenType.CloseParent;
                idEnd++;
            }
            else if (currentString[idStart] == ',')
            {
                token.Lexeme = currentString[idStart].ToString();
                token.Type = TokenType.Comma;
                idEnd++;
            }
            else if (currentString[idStart] == '"')
            {
                token.Lexeme = GetStringLiteralFromString(currentString);
                token.Type = TokenType.String;
                idEnd++;
            }
            else
            {
                token.Lexeme = null;
                token.Type = TokenType.Unknown;
            }
        }

        //Debug.Log(token.Lexeme + " - " + token.Type);

        return token;
    }

    private string GetLexemeFromString(string str)
    {
        string lexeme = "";
        
        idEnd = idStart;

        while (idEnd != str.Length)
        {
            lexeme += str[idEnd++];
            
            if (idEnd >= str.Length || IsSeparator(str[idEnd]))
            {
                break;
            }
        }

        return lexeme;
    }

    private string GetStringLiteralFromString(string str)
    {
        string lexeme = "";
        
        idEnd = idStart;

        if (str[idEnd] == '"') // Skip starting quote
            idEnd++;

        while (idEnd != str.Length)
        {
            lexeme += str[idEnd++];

            if (idEnd >= str.Length || str[idEnd] == '"')
            {
                break;
            }
        }

        return lexeme;
    }

    // El retorno de carro puede ser diferente en 
    // cada sistema operativo
    private bool IsCarriageReturn(char ch)
    {
        return ch == '\n' || ch == '\r' || ch == '\a';
    } 

    private bool IsSeparator(char ch)
    {
        return !char.IsLetterOrDigit(ch);
    }

    private string RemoveSpaces(string str)
    {
        bool isInsideString = false;
        string finalString = "";
        
        for (int i = 0; i < str.Length; i++)
        {
            char ch = str[i];

            if (isInsideString)
            {
                finalString += ch;
                if (ch == '"')
                    isInsideString = false;
            }
            else 
            {
                if (!char.IsWhiteSpace(ch) || ch == '\a' || ch == '\r' || ch == '\n')
                    finalString += ch;
                
                if (ch == '"')
                    isInsideString = true;
            }
        }

        return finalString;
    }
}
