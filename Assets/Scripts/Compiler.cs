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
		// ------------------------------------------------------------- 1
		tables.AddInstrLookUp("ADD", OpCodes.INSTR_ADD, 2);
		tables.SetOpType("ADD", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("ADD", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 2
		tables.AddInstrLookUp("SUB", OpCodes.INSTR_SUB, 2);
		tables.SetOpType("SUB", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("SUB", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 3
		tables.AddInstrLookUp("MUL", OpCodes.INSTR_MUL, 2);
		tables.SetOpType("MUL", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("MUL", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 4
		tables.AddInstrLookUp("DIV", OpCodes.INSTR_DIV, 2);
		tables.SetOpType("DIV", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("DIV", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 5
		tables.AddInstrLookUp("MOD", OpCodes.INSTR_MOD, 2);
		tables.SetOpType("MOD", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("MOD", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 6
		tables.AddInstrLookUp("EXP", OpCodes.INSTR_EXP, 1);
		tables.SetOpType("EXP", 0, 
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 7
		tables.AddInstrLookUp("NEG", OpCodes.INSTR_NEG, 1);
		tables.SetOpType("NEG", 0, 
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 8
		tables.AddInstrLookUp("INC", OpCodes.INSTR_INC, 1);
		tables.SetOpType("INC", 0, 
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 9
		tables.AddInstrLookUp("DEC", OpCodes.INSTR_DEC, 1);
		tables.SetOpType("DEC", 0, 
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 10
		tables.AddInstrLookUp("AND", OpCodes.INSTR_AND, 2);
		tables.SetOpType("AND", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("AND", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 11
		tables.AddInstrLookUp("OR", OpCodes.INSTR_OR, 2);
		tables.SetOpType("OR", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("OR", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 12
		tables.AddInstrLookUp("XOR", OpCodes.INSTR_XOR, 2);
		tables.SetOpType("XOR", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("XOR", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 13
		tables.AddInstrLookUp("NOT", OpCodes.INSTR_NOT, 1);
		tables.SetOpType("NOT", 0, 
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 14
		tables.AddInstrLookUp("SHL", OpCodes.INSTR_SHL, 2);
		tables.SetOpType("SHL", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("SHL", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 15
		tables.AddInstrLookUp("SHR", OpCodes.INSTR_SHR, 2);
		tables.SetOpType("SHR", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("SHR", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 16
		tables.AddInstrLookUp("JMP", OpCodes.INSTR_JMP, 1);
		tables.SetOpType("JMP", 0, 
			OpFlags.InstrIdx
		);
		// ------------------------------------------------------------- 17
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
		// ------------------------------------------------------------- 18
		tables.AddInstrLookUp("JNE", OpCodes.INSTR_JNE, 3);
		tables.SetOpType("JNE", 0, 
			OpFlags.InstrIdx
		);
		tables.SetOpType("JNE", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		tables.SetOpType("JNE", 2, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 19
		tables.AddInstrLookUp("JG", OpCodes.INSTR_JG, 3);
		tables.SetOpType("JG", 0, 
			OpFlags.InstrIdx
		);
		tables.SetOpType("JG", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		tables.SetOpType("JG", 2, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 20
		tables.AddInstrLookUp("JL", OpCodes.INSTR_JL, 3);
		tables.SetOpType("JL", 0, 
			OpFlags.InstrIdx
		);
		tables.SetOpType("JL", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		tables.SetOpType("JL", 2, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 21
		tables.AddInstrLookUp("JGE", OpCodes.INSTR_JGE, 3);
		tables.SetOpType("JGE", 0, 
			OpFlags.InstrIdx
		);
		tables.SetOpType("JGE", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		tables.SetOpType("JGE", 2, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 22
		tables.AddInstrLookUp("JLE", OpCodes.INSTR_JLE, 3);
		tables.SetOpType("JLE", 0, 
			OpFlags.InstrIdx
		);
		tables.SetOpType("JLE", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		tables.SetOpType("JLE", 2, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 23
		tables.AddInstrLookUp("PUSH", OpCodes.INSTR_PUSH, 1);
		tables.SetOpType("PUSH", 0, 
			OpFlags.MemIdx |
			OpFlags.Literal 
		);
		// ------------------------------------------------------------- 24
		tables.AddInstrLookUp("POP", OpCodes.INSTR_POP, 1);
		tables.SetOpType("POP", 0, 
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 25
		tables.AddInstrLookUp("PAUSE", OpCodes.INSTR_PAUSE, 1);
		tables.SetOpType("PAUSE", 0, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);
		// ------------------------------------------------------------- 26
		tables.AddInstrLookUp("EXIT", OpCodes.INSTR_EXIT, 0);
		// ------------------------------------------------------------- 27
		tables.AddInstrLookUp("JSR", OpCodes.INSTR_JSR, 1);
		tables.SetOpType("JSR", 0, 
			OpFlags.FuncIdx
		);
		// ------------------------------------------------------------- 28
		tables.AddInstrLookUp("RET", OpCodes.INSTR_RET, 0);
		// ------------------------------------------------------------- 29
		tables.AddInstrLookUp("CLH", OpCodes.INSTR_CALLHOST, 2);
		tables.SetOpType("CLH", 0, 
			OpFlags.HostAPICallIdx
		);
		tables.SetOpType("CLH", 1, 
			OpFlags.Literal
		);
		// ------------------------------------------------------------- 30
		tables.AddInstrLookUp("LN", OpCodes.INSTR_LN, 1);
		tables.SetOpType("LN", 0, 
			OpFlags.InstrIdx
		);
		// ------------------------------------------------------------- 31
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
