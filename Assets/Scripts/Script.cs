using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Script{
	enum State{
		None,
		Running,
		Pause,
	}

	private delegate void InstrDlg(Value[] values);

	ScriptContext context;
	ErrorManager errorHandler;

	InstrDlg[] instrExec = new InstrDlg[OpCodes.COUNT];
	State state = State.None;

	public Script(){
		errorHandler = null;
		SetInstrDlg();
	}

	public Script(ScriptContext context){
		this.context = context;
		errorHandler = null;
		SetInstrDlg();
	}

	public Script(ScriptContext context, ErrorManager errorHandler){
		this.context = context;
		this.errorHandler = errorHandler;
		SetInstrDlg();
	}

	public void HostAPIFunctionRegister(HostFuncsDlg func){
		HostFuncs newFunc;
		
		newFunc.ident = func.Method.Name.ToUpper();
		newFunc.func = func;
		
		context.hostFuncs.Add(newFunc);
	}

	private void SetInstrDlg(){
		instrExec[OpCodes.INSTR_MOV] = InstrMov;
		instrExec[OpCodes.INSTR_ADD] = InstrAdd;
		instrExec[OpCodes.INSTR_EXIT] = InstrExit;
		instrExec[OpCodes.INSTR_JMP] = InstrJmp;
		instrExec[OpCodes.INSTR_JE] = InstrJe;
		instrExec[OpCodes.INSTR_PAUSE] = InstrPause;
		instrExec[OpCodes.INSTR_POP] = InstrPop;
		instrExec[OpCodes.INSTR_PUSH] = InstrPush;
		instrExec[OpCodes.INSTR_SUB] = InstrSub;
		instrExec[OpCodes.INSTR_CALLHOST] = CallHost;
		instrExec[OpCodes.INSTR_MUL] = InstrMul;
		instrExec[OpCodes.INSTR_DIV] = InstrDiv;
		instrExec[OpCodes.INSTR_MOD] = InstrMod;
		instrExec[OpCodes.INSTR_EXP] = InstrExp;
		instrExec[OpCodes.INSTR_NEG] = InstrNeg;
		instrExec[OpCodes.INSTR_INC] = InstrInc;
		instrExec[OpCodes.INSTR_DEC] = InstrDec;
		instrExec[OpCodes.INSTR_AND] = InstrAnd;
		instrExec[OpCodes.INSTR_OR] = InstrOr;
		instrExec[OpCodes.INSTR_XOR] = InstrXor;
		instrExec[OpCodes.INSTR_NOT] = InstrNot;
		instrExec[OpCodes.INSTR_SHL] = InstrShl;
		instrExec[OpCodes.INSTR_SHR] = InstrShr;
		instrExec[OpCodes.INSTR_JSR] = InstrJsr;
		instrExec[OpCodes.INSTR_RET] = InstrRet;
		
	}

	public void RunStep(){
		if (state == State.Running)
		{
			if (context.instrStream.Instructions == null)
				return;

			if (context.instrStream.PC >= 0 && context.instrStream.PC < context.instrStream.Instructions.Length)
			{
				Instruction op = context.instrStream.Instructions[context.instrStream.PC];

				if (instrExec[op.OpCode] != null)
					instrExec[op.OpCode](op.Values);

				context.instrStream.PC++;
			}
			else
			{
				Stop();
			}
		}
	}

	void ResetContext(){
		context.instrStream.PC = context.instrStream.StartPC;
		context.stack.TopStackIdx = 0;
	}

	public void Start(){
		if (state == State.Running)
			ResetContext();
			
		state = State.Running;
	}

	public void Stop(){
		state = State.None;
		ResetContext();
	}

	public void Pause(bool pause){
		if (pause)
		{
			state = State.Pause;
		}
		else
		{
			state = State.Running;
		}
	}

#region Instructions
	private void InstrMov(Value[] values)
	{
		Value val = ResolveOpValue(values[1]);
		ResolveOpValueAndSet(0, val);
	}

	private void InstrAdd(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);

		switch(val1.Type)
		{
			case OpType.Int:
				if (val2.Type == OpType.Int)
					val1.IntLiteral += val2.IntLiteral;
				else if (val2.Type == OpType.Float)
				{
					val1.Type = OpType.Float;
					val1.FloatLiteral = (float)val1.IntLiteral + val2.FloatLiteral;
				}
				else if (val2.Type == OpType.String)
				{
					val1.Type = OpType.String;
					val1.StringLiteral = val1.IntLiteral + val2.StringLiteral;
				}
			break;
			case OpType.Float:
				if (val2.Type == OpType.Int)
					val1.FloatLiteral += (float)val2.IntLiteral;
				else if (val2.Type == OpType.Float)
					val1.FloatLiteral += val2.FloatLiteral;
				else if (val2.Type == OpType.String)
				{
					val1.Type = OpType.String;
					val1.StringLiteral = val1.FloatLiteral + val2.StringLiteral;
				}
			break;
			case OpType.String:
				if (val2.Type == OpType.Int)
					val1.StringLiteral = val1.StringLiteral + val2.IntLiteral;
				else if (val2.Type == OpType.Float)
					val1.StringLiteral = val1.StringLiteral + val2.FloatLiteral;
				else if (val2.Type == OpType.String)
					val1.StringLiteral = val1.StringLiteral + val2.StringLiteral;
			break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}

	private void InstrSub(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);

		switch(val1.Type)
		{
			case OpType.Int:
				if (val2.Type == OpType.Int)
					val1.IntLiteral -= val2.IntLiteral;
				else if (val2.Type == OpType.Float)
				{
					val1.Type = OpType.Float;
					val1.FloatLiteral = (float)val1.IntLiteral - val2.FloatLiteral;
				}
			break;
			case OpType.Float:
				if (val2.Type == OpType.Int)
					val1.FloatLiteral -= (float)val2.IntLiteral;
				else if (val2.Type == OpType.Float)
					val1.FloatLiteral -= val2.FloatLiteral;
			break;
			case OpType.String:
			break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}
	
	private void InstrMul(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);

		switch(val1.Type)
		{
			case OpType.Int:
				if (val2.Type == OpType.Int)
					val1.IntLiteral *= val2.IntLiteral;
				else if (val2.Type == OpType.Float)
				{
					val1.Type = OpType.Float;
					val1.FloatLiteral = (float)val1.IntLiteral * val2.FloatLiteral;
				}
			break;
			case OpType.Float:
				if (val2.Type == OpType.Int)
					val1.FloatLiteral *= (float)val2.IntLiteral;
				else if (val2.Type == OpType.Float)
					val1.FloatLiteral *= val2.FloatLiteral;
			break;
			case OpType.String:
			break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}

	private void InstrDiv(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);

		switch(val1.Type)
		{
			case OpType.Int:
				val1.Type = OpType.Float;
				val1.FloatLiteral = (float)val1.IntLiteral;
				if (val2.Type == OpType.Int)
					if (val2.IntLiteral == 0){
						val1.Type = OpType.String;
						val1.StringLiteral = "ERROR | division by 0";
					}
					else
						val1.FloatLiteral /= (float)val2.IntLiteral;
				else if (val2.Type == OpType.Float)
				{
					if (val2.FloatLiteral == 0.0f){
						val1.Type = OpType.String;
						val1.StringLiteral = "ERROR | division by 0";
					}
					else
						val1.FloatLiteral /= val2.FloatLiteral;
				}
			break;
			case OpType.Float:
				if (val2.Type == OpType.Int)
					if (val2.IntLiteral == 0){
						val1.Type = OpType.String;
						val1.StringLiteral = "ERROR | division by 0";
					}
					else
						val1.FloatLiteral /= (float)val2.IntLiteral;
				else if (val2.Type == OpType.Float)
					if (val2.FloatLiteral == 0.0f){
						val1.Type = OpType.String;
						val1.StringLiteral = "ERROR | division by 0";
					}
					else
						val1.FloatLiteral /= val2.FloatLiteral;
			break;
			case OpType.String:
			break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}

	private void InstrMod(Value[] values){
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);

		switch (val1.Type)
		{
			case OpType.Int:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral %= val2.IntLiteral;
						break;
					case OpType.Float:
						val1.FloatLiteral = (float)val1.IntLiteral % val2.FloatLiteral;
						val1.Type = OpType.Float;
						break;
					case OpType.String:
						break;
				}
				
				break;
			case OpType.Float:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.FloatLiteral = val1.IntLiteral % (float)val2.IntLiteral;
						break;
					case OpType.Float:
						val1.FloatLiteral %= val2.FloatLiteral;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.String:
				break;
		}

		ResolveOpValueAndSet(0, val1);
	}

	private void InstrExp(Value[] values)
	{
		Value val = ResolveOpValue(values[0]);
		
		switch(val.Type)
		{
			case OpType.Int:
				Mathf.Exp(val.IntLiteral);	
				break;
			case OpType.Float:
				Mathf.Exp(val.FloatLiteral);
				break;
			case OpType.String:
			break;
		}
		
		ResolveOpValueAndSet(0, val);
	}

	private void InstrNeg(Value[] values)
	{
		Value val = ResolveOpValue(values[0]);
		
		switch(val.Type)
		{
			case OpType.Int:
				val.IntLiteral *= -1;	
				break;
			case OpType.Float:
				val.FloatLiteral *= -1;
				break;
			case OpType.String:
			break;
		}
		
		ResolveOpValueAndSet(0, val);
	}

	private void InstrInc(Value[] values)
	{
		Value val = ResolveOpValue(values[0]);
		
		switch(val.Type)
		{
			case OpType.Int:
				val.IntLiteral++;	
				break;
			case OpType.Float:
				val.FloatLiteral++;
				break;
			case OpType.String:
			break;
		}
		
		ResolveOpValueAndSet(0, val);
	}

	private void InstrDec(Value[] values)
	{
		Value val = ResolveOpValue(values[0]);
		
		switch(val.Type)
		{
			case OpType.Int:
				val.IntLiteral--;	
				break;
			case OpType.Float:
				val.FloatLiteral--;
				break;
			case OpType.String:
			break;
		}
		
		ResolveOpValueAndSet(0, val);
	}

	private void InstrAnd(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);
		
		switch(val1.Type)
		{
			case OpType.Int:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral &= val2.IntLiteral;
						break;
					case OpType.Float:
						val1.IntLiteral &= (int)val2.FloatLiteral;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.Float:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral = (int)val2.FloatLiteral & val2.IntLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.Float:
						val1.IntLiteral = (int)val1.FloatLiteral & (int)val2.FloatLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.String:
				break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}

	private void InstrOr(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);
		
		switch(val1.Type)
		{
			case OpType.Int:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral |= val2.IntLiteral;
						break;
					case OpType.Float:
						val1.IntLiteral |= (int)val2.FloatLiteral;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.Float:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral = (int)val2.FloatLiteral | val2.IntLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.Float:
						val1.IntLiteral = (int)val1.FloatLiteral | (int)val2.FloatLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.String:
				break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}

	private void InstrXor(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);
		
		switch(val1.Type)
		{
			case OpType.Int:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral ^= val2.IntLiteral;
						break;
					case OpType.Float:
						val1.IntLiteral ^= (int)val2.FloatLiteral;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.Float:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral = (int)val2.FloatLiteral ^ val2.IntLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.Float:
						val1.IntLiteral = (int)val1.FloatLiteral ^ (int)val2.FloatLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.String:
				break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}

	private void InstrNot(Value[] values)
	{
		Value val = ResolveOpValue(values[0]);
		
		switch(val.Type)
		{
			case OpType.Int:
					val.IntLiteral = ~val.IntLiteral;
				break;
			case OpType.Float:
					val.IntLiteral = ~(int)val.FloatLiteral;
					val.Type = OpType.Int;
				break;
			case OpType.String:
				break;
		}
		
		ResolveOpValueAndSet(0, val);
	}

	private void InstrShl(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);
		
		switch(val1.Type)
		{
			case OpType.Int:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral = val1.IntLiteral << val2.IntLiteral;
						break;
					case OpType.Float:
						val1.IntLiteral = val1.IntLiteral << (int)val2.FloatLiteral;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.Float:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral = (int)val1.FloatLiteral << val2.IntLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.Float:
						val1.IntLiteral = (int)val1.FloatLiteral << (int)val2.FloatLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.String:
				break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}

private void InstrShr(Value[] values)
	{
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);
		
		switch(val1.Type)
		{
			case OpType.Int:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral = val1.IntLiteral >> val2.IntLiteral;
						break;
					case OpType.Float:
						val1.IntLiteral = val1.IntLiteral >> (int)val2.FloatLiteral;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.Float:
				switch (val2.Type)
				{
					case OpType.Int:
						val1.IntLiteral = (int)val1.FloatLiteral >> val2.IntLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.Float:
						val1.IntLiteral = (int)val1.FloatLiteral >> (int)val2.FloatLiteral;
						val1.Type = OpType.Int;
						break;
					case OpType.String:
						break;
				}
				break;
			case OpType.String:
				break;
		}
		
		ResolveOpValueAndSet(0, val1);
	}

	private void InstrJmp(Value[] values){
		Value val1 = ResolveOpValue(values[0]);
		context.instrStream.PC = val1.IntLiteral - 1;
	}

	private void InstrJe(Value[] values){
		Value val1 = ResolveOpValue(values[0]);
		Value val2 = ResolveOpValue(values[1]);
		Value val3 = ResolveOpValue(values[2]);
		
		bool shouldJump = false;

		switch(val2.Type)
		{
			case OpType.Int:
				if (val3.Type == OpType.Int)
					shouldJump = val2.IntLiteral == val3.IntLiteral;
				else if (val3.Type == OpType.Float)
					shouldJump = val2.IntLiteral == (int)val3.FloatLiteral;
				else if(val3.Type == OpType.String)
					shouldJump = val2.IntLiteral.ToString() == val3.StringLiteral;
			break;
			case OpType.Float:
				if (val3.Type == OpType.Int)
					shouldJump = val2.FloatLiteral == val3.IntLiteral;
				else if(val3.Type == OpType.Float)
					shouldJump = val2.FloatLiteral == val3.FloatLiteral;
				else if(val3.Type == OpType.String)
					shouldJump = val2.FloatLiteral.ToString() == val3.StringLiteral;
			break;
			case OpType.String:
				if (val3.Type == OpType.Int)
					shouldJump = val2.StringLiteral == val3.IntLiteral.ToString();
				else if(val3.Type == OpType.Float)
					shouldJump = val2.StringLiteral == val3.FloatLiteral.ToString();
				else if (val3.Type == OpType.String)
					shouldJump = val2.StringLiteral == val3.StringLiteral;
			break;
		}

		if (shouldJump)
			context.instrStream.PC = val1.IntLiteral - 1;
	}

	private void InstrPush(Value[] values){
		Value val = ResolveOpValue(values[0]);
		Push(val);
	}

	private void InstrPop(Value[] values){
		Value val = Pop();
		ResolveOpValueAndSet(0, val);
	}

	private void InstrExit(Value[] values){
		Stop();
	}

	private void InstrPause(Value[] values){
		Value val = ResolveOpValue(values[0]);

		Pause(val.IntLiteral > 0);
	}

#endregion 

	private void Push(Value val){
		int start = context.stack.StackStartIdx;
		int top = context.stack.TopStackIdx;
		int idx = start + top;

		if (idx < context.stack.Elements.Length){
			context.stack.Elements[idx] = val;
			context.stack.TopStackIdx++;
		}
		else{
			if (errorHandler != null)
				errorHandler.RunTimeLogError("Stack Overflow");
		}
	}

	private Value Pop(){
		int start = context.stack.StackStartIdx;
		int top = start + context.stack.TopStackIdx;
		int idx = top - 1;
		
		Value val; 

		if (idx >= start){
			val = context.stack.Elements[idx];
			context.stack.TopStackIdx--;
		}
		else{
			val = new Value();

			if (errorHandler != null)
				errorHandler.RunTimeLogError("Stack Overflow");
		}

		return val;
	}

	private void InstrJsr(Value[] values){
		Value val = ResolveOpValue(values[0]);
		CallStack ret = new CallStack();
		ret.ReturnIdx = context.instrStream.PC;
		ret.ReturnTopStackIdx = context.stack.TopStackIdx;
		context.CallStack.Push(ret);
		int instIdx = context.Funcs[val.IntLiteral].StartIdx - 1;
		context.stack.TopStackIdx += context.Funcs[val.IntLiteral].frameSize;
		context.instrStream.PC = instIdx;
	}

	private void InstrRet(Value[] values){
		CallStack top = context.CallStack.Pop();
		context.instrStream.PC = top.ReturnIdx;
		context.stack.TopStackIdx = top.ReturnTopStackIdx;
	}

	private Value ResolveOpValue(Value val){
		switch(val.Type){
			case OpType.AbsMemIdx:
				return context.stack.Elements[val.IntLiteral];
			case OpType.RelMemIdx:
				CallStack call =  context.CallStack.Peek();
				return context.stack.Elements[val.IntLiteral + call.ReturnTopStackIdx + context.stack.StackStartIdx];
			default:
				return val;
		}
	}

	private void ResolveOpValueAndSet(int idx, Value val){
		Value dst = GetOpValue(idx);

		switch(dst.Type){
			case OpType.AbsMemIdx:
				context.stack.Elements[dst.IntLiteral] = val;
				break;
			case OpType.RelMemIdx:
				CallStack call =  context.CallStack.Peek();
				context.stack.Elements[dst.IntLiteral + call.ReturnTopStackIdx + context.stack.StackStartIdx] = val;
				break;
		}
	}

	private Value GetOpValue(int idx){
		return context.instrStream.Instructions[context.instrStream.PC].Values[idx];
	}

	private void CallHost(Value[] values){
		Value[] args = values;
		switch(values[0].Type){
			
			case OpType.HostAPICallString:
				values[0].StringLiteral = values[0].StringLiteral.ToUpper();
				
				if (values[1].Type == OpType.Int){
					
					for (int i = 0; i < context.hostFuncs.Count; i++){
						
						if (context.hostFuncs[i].ident == values[0].StringLiteral){
							
							values[0].IntLiteral = i;
							values[0].Type = OpType.HostAPICallIdx;
							
							context.instrStream.Instructions[context.instrStream.PC].Values[0] = values[0];

							if (values[1].IntLiteral > 0){
								args = new Value[values[1].IntLiteral];
							
								for (int j = 0; j < values[1].IntLiteral; j++){
									args[j] = Pop();
								}
							}

							context.hostFuncs[values[0].HostAPICallIndex].func(args);
							break;
						}
					}
				}
				else{
					errorHandler.RunTimeLogError("Unexpected Parameter for Function");
				}
				break;
			case OpType.HostAPICallIdx:
				if (values[1].Type == OpType.Int){
					if (values[1].IntLiteral > 0){
						
						args = new Value[values[1].IntLiteral];
						
						for (int i = 0; i < values[1].IntLiteral; i++){
							args[i] = Pop();
						}
					}

					context.hostFuncs[values[0].HostAPICallIndex].func(args);
				}

				break;
		}
	}
}
