using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compiler 
{
	Tables tables = new Tables();
	Parser parser;

	public Compiler()
	{
		parser = new Parser(tables);

		tables.AddInstrLookUp("NOP", OpCodes.NOP);
		tables.AddInstrLookUp("LOG", OpCodes.LOG);
		tables.AddInstrLookUp("GOTO", OpCodes.GOTO);
	}

	public bool Compile(string str, out List<Instruction> instructions)
	{
		parser.Reset();


		if (!parser.Parse(str, out instructions))
		{
			Debug.Log("Error while parsing...");
			return false;		
		}


		foreach(Instruction i in instructions)
		{
			string dbg = i.OpCode + " ";

			if (i.Arguments != null && i.Arguments.Count > 0)
			{
				foreach(string s in i.Arguments)
				{
					dbg += s + " ";
				}
			}

			Debug.Log(dbg);
		}


		return true;
	}



}
