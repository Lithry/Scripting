using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMachine 
{
	List<Instruction> program;
	private int PC = 0; // Program counter

	public void Reset(List<Instruction> program)
	{
		this.program = program;
		PC = 0;
	}

	public void RunStep()
	{
		if (program != null && PC >= 0 && PC < program.Count)
		{
			Instruction op = program[PC];

			switch(op.OpCode)
			{
				case OpCodes.LOG:
					Log(op.Arguments);
				break;

				case OpCodes.GOTO:
					GoTo(op.Arguments);
				break;

				case OpCodes.NOP:
				break;
			}

			PC++;
		}
	}

	void Log(List<string> args)
	{
		foreach(string s in args)
		{
			Debug.Log(s);
		}
	}

	void GoTo(List<string> args)
	{
		if (args.Count > 0)
		{
			int jmpIdx = -1;

			if (int.TryParse(args[0], out jmpIdx))
				PC = jmpIdx;
		}		
	}
	
}
