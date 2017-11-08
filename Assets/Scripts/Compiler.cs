using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Compiler{
	Tables tables = new Tables();
	Parser parser;
	ErrorManager errorHandler;

	public Compiler(ErrorManager errorHandler){
		this.errorHandler = errorHandler;
		parser = new Parser(tables, errorHandler);

		tables.AddInstrLookUp("MOV", OpCodes.INSTR_MOV, 2);
		tables.SetOpType("MOV", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("MOV", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("ADD", OpCodes.INSTR_ADD, 2);
		tables.SetOpType("ADD", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("ADD", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("SUB", OpCodes.INSTR_SUB, 2);
		tables.SetOpType("SUB", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("SUB", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		
		tables.AddInstrLookUp("MUL", OpCodes.INSTR_MUL, 2);
		tables.SetOpType("MUL", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("MUL", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("DIV", OpCodes.INSTR_DIV, 2);
		tables.SetOpType("DIV", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("DIV", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("EXP", OpCodes.INSTR_EXP, 2);
		tables.SetOpType("EXP", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("EXP", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("JMP", OpCodes.INSTR_JMP, 1);
		tables.SetOpType("JMP", 0, 
			OpFlags.InstrIdx
		);

		tables.AddInstrLookUp("JE", OpCodes.INSTR_JE, 3);
		tables.SetOpType("JE", 0, 
			OpFlags.InstrIdx
		);
		tables.SetOpType("JE", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		tables.SetOpType("JE", 2, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("EXIT", OpCodes.INSTR_EXIT, 0);

		tables.AddInstrLookUp("PAUSE", OpCodes.INSTR_PAUSE, 1);
		tables.SetOpType("PAUSE", 0, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("POP", OpCodes.INSTR_POP, 1);
		tables.SetOpType("POP", 0, 
			OpFlags.MemIdx
		);
		
		tables.AddInstrLookUp("PUSH", OpCodes.INSTR_PUSH, 1);
		tables.SetOpType("PUSH", 0, 
			OpFlags.MemIdx |
			OpFlags.Literal 
		);

		tables.AddInstrLookUp("CLH", OpCodes.INSTR_CALLHOST, 2);
		tables.SetOpType("CLH", 0, 
			OpFlags.HostAPICallIdx
		);
		tables.SetOpType("CLH", 1, 
			OpFlags.Literal
		);

		tables.AddInstrLookUp("JSR", OpCodes.INSTR_JSR, 1);
		tables.SetOpType("JSR", 0, 
			OpFlags.FuncIdx
		);

		tables.AddInstrLookUp("RET", OpCodes.INSTR_RET, 0);
	}

	public Tables GetTables(){
		return tables;
	}

	public bool Compile(string str){
		parser.Reset();
		
		if (!parser.Parse(str)){
			return false;		
		}

		return true;
	}

}
