using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Compiler 
{
	Tables tables = new Tables();
	Parser parser;

	public Compiler()
	{
		parser = new Parser(tables);

		tables.AddInstrLookUp("MOV", OpCodes.INSTR_MOV, 2);
		tables.SetOpType("MOV", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("MOV", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("JMP", OpCodes.INSTR_JMP, 1);
		tables.SetOpType("JMP", 0, 
			OpFlags.InstrIdx
		);
	}

	public Tables GetTables()
	{
		return tables;
	}

	public bool Compile(string str)
	{
		parser.Reset();
		
		if (!parser.Parse(str))
		{
			Debug.Log("Error while parsing...");
			return false;		
		}

		return true;
	}

}
