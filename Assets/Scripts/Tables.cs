using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tables
{
	List<Label> labelsTable = new List<Label>();
	Dictionary<string, int> instrLookUp = new Dictionary<string, int>();

	public void Clear()
	{
		labelsTable.Clear();
	}

	public bool AddLabel(string ident, int idx)
	{
		Label label;
		
		if (GetLabelByName(ident, out label)) // Already exists!
			return false;

		label = new Label();

		label.Ident = ident;
		label.Index = idx;

		labelsTable.Add(label);

		return true;
	}	

	public bool GetLabelByName(string name, out Label label)
	{
		for(int i = 0; i < labelsTable.Count; i++)
		{
			if (labelsTable[i].Ident == name)
			{
				label = labelsTable[i];
				return true;
			}	
		}
		
		label = new Label();

		return false;
	}

	public bool AddInstrLookUp(string instruction, int opcode)
	{
		instruction = instruction.ToUpper();

		if (instrLookUp.ContainsKey(instruction))
			return false;

		instrLookUp[instruction] = opcode;

		return true;
	}

	public bool GetInstrLookUp(string instruction, out int opcode)
	{
		instruction = instruction.ToUpper();
		
		opcode = -1;

		if (!instrLookUp.ContainsKey(instruction))
			return false;

		opcode = instrLookUp[instruction];

		return true;		
	}
}
