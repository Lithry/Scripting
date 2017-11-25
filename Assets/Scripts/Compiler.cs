﻿using System.Collections;
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

		tables.AddInstrLookUp("MOD", OpCodes.INSTR_MOD, 2);
		tables.SetOpType("MOD", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("MOD", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("EXP", OpCodes.INSTR_EXP, 1);
		tables.SetOpType("EXP", 0, 
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("NEG", OpCodes.INSTR_NEG, 1);
		tables.SetOpType("NEG", 0, 
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("INC", OpCodes.INSTR_INC, 1);
		tables.SetOpType("INC", 0, 
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("DEC", OpCodes.INSTR_DEC, 1);
		tables.SetOpType("DEC", 0, 
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("AND", OpCodes.INSTR_AND, 2);
		tables.SetOpType("AND", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("AND", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("OR", OpCodes.INSTR_OR, 2);
		tables.SetOpType("OR", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("OR", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("XOR", OpCodes.INSTR_XOR, 2);
		tables.SetOpType("XOR", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("XOR", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("NOT", OpCodes.INSTR_NOT, 1);
		tables.SetOpType("NOT", 0, 
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("SHL", OpCodes.INSTR_SHL, 2);
		tables.SetOpType("SHL", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("SHL", 1, 
			OpFlags.Literal |
			OpFlags.MemIdx
		);

		tables.AddInstrLookUp("SHR", OpCodes.INSTR_SHR, 2);
		tables.SetOpType("SHR", 0, 
			OpFlags.MemIdx
		);
		tables.SetOpType("SHR", 1, 
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
