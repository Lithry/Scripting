using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Label
{
	public int Index;
	public string Ident;
}

public struct Instruction
{
	public int OpCode;
	public List<string> Arguments;
}

public static class OpCodes
{
	public const int NOP = 0;
	public const int LOG = 1;
	public const int GOTO = 2;
}