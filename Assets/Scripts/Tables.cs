using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tables
{
	List<LabelDecl> labelsTable = new List<LabelDecl>();
	Dictionary<string, InstrDecl> instrLookUp = new Dictionary<string, InstrDecl>();
	List<VarDecl> varsTable = new List<VarDecl>();
	List<FuncDecl> funcTable = new List<FuncDecl>();
	List<Instruction> instrStream = new List<Instruction>();
	int startPC;
	int globalVarSize = 0;

	int varIdx = 0;

	public void Clear()
	{
		instrStream.Clear();
		labelsTable.Clear();
		varsTable.Clear();
		funcTable.Clear();
		startPC = -1;
		globalVarSize = 0;
		varIdx = 0;
	}

	public bool AddLabel(string ident, int idx, int scope)
	{
		LabelDecl label;
		
		if (GetLabelByName(ident, out label, scope)) // Already exists!
			return false;

		label = new LabelDecl();

		label.Ident = ident;
		label.Idx = idx;
		label.scope = scope;

		labelsTable.Add(label);

		return true;
	}	

	public bool GetLabelByName(string name, out LabelDecl label, int scope)
	{
		for(int i = 0; i < labelsTable.Count; i++)
		{
			if (labelsTable[i].Ident == name && labelsTable[i].scope == scope)
			{
				label = labelsTable[i];
				return true;
			}	
		}
		
		label = new LabelDecl();

		return false;
	}
	
	public bool AddVar(string ident, int scope, bool isArgument)
	{
		VarDecl var;

		if (GetVarByIdent(ident, out var, scope))
			return false;

		var = new VarDecl();

		var.Ident = ident;
		if (scope == -1)
		{
			var.Idx = varIdx;
			varIdx++;
		}
		else
		{
			FuncDecl func = funcTable[scope];
			if (isArgument)
			{
				var.Idx = func.argIdx;
				func.argIdx++;
			}
			else
			{
				var.Idx = func.varIdx;
				func.varIdx++;
			}
			var.isArg = isArgument;
			funcTable[scope] = func;
		}

		var.scope = scope;

		if (scope == -1)
			globalVarSize++;

		varsTable.Add(var);

		return true;
	}
	
	public bool GetVarByIdent(string ident, out VarDecl varDecl, int scope)
	{
		for(int i = 0; i < varsTable.Count; i++)
		{
			if (varsTable[i].Ident == ident && (varsTable[i].scope == -1 || varsTable[i].scope == scope))
			{
				varDecl = varsTable[i];
				return true;
			}	
		}

		varDecl = new VarDecl();

		return false;
	}

	public List<VarDecl> GetVarsTable()
	{
		return varsTable;
	}

	public int GetGlobalVarSize()
	{
		return globalVarSize;
	}

	public bool AddFunc(string ident, int idx, out int scope){
		scope = -1;
		FuncDecl func;

		if (GetFuncByIdent(ident, out func))
			return false;

		func = new FuncDecl();

		func.Ident = ident;
		func.StartIdx = idx;
		scope = func.scope = funcTable.Count;
		funcTable.Add(func);

		return true;
	}

	public bool GetFuncByIdent(string ident, out FuncDecl funcDecl){
		for(int i = 0; i < funcTable.Count; i++)
		{
			if (funcTable[i].Ident == ident)
			{
				funcDecl = funcTable[i];
				return true;
			}	
		}

		funcDecl = new FuncDecl();

		return false;
	}

	public bool FuncIncrementFrameSize(int scope){
		FuncDecl func = funcTable[scope];
		func.frameSize += 1;
		funcTable[scope] = func;

		return true;
	}
	
	public bool FuncIncrementArgFrameSize(int scope){
		FuncDecl func = funcTable[scope];
		func.argFrameSize += 1;
		funcTable[scope] = func;

		return true;
	}

	public bool AddInstrToStream(Instruction instruction)
	{
		instrStream.Add(instruction);

		return true;
	}

	public List<Instruction> GetInstrStream()
	{
		return instrStream;
	}

	public List<FuncDecl> GetFunctions(){
		return funcTable;
	}

	public bool AddInstrLookUp(string instruction, int opcode, int argsCount)
	{
		instruction = instruction.ToUpper();

		if (instrLookUp.ContainsKey(instruction))
			return false;

		InstrDecl instrDecl = new InstrDecl();
		instrDecl.OpCode = opcode;
		instrDecl.ParamsCount = argsCount;
		
		if (argsCount > 0)
			instrDecl.ParamsFlags = new int[argsCount];

		instrLookUp[instruction] = instrDecl;

		return true;
	}

	public bool GetInstrLookUp(string instruction, out InstrDecl instrDecl)
	{
		instruction = instruction.ToUpper();
		
		instrDecl = new InstrDecl();

		if (!instrLookUp.ContainsKey(instruction))
			return false;

		instrDecl = instrLookUp[instruction];

		return true;		
	}

	public bool SetOpType(string instruction, int argNum, int flags)
	{
		if (!instrLookUp.ContainsKey(instruction))
			return false;

		InstrDecl instrDecl = instrLookUp[instruction];

		if (argNum >= 0 && argNum < instrDecl.ParamsCount)
		{
			instrDecl.ParamsFlags[argNum] = flags;
		}
		else
		{
			return false;
		}

		instrLookUp[instruction] = instrDecl;
		
		return true;
	}

	public int GetStartPC(){
		return startPC;
	}

	public void SetStartPC(int PC){
		startPC = PC;
	}
}
