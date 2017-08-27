using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour {
	static public Console instance;
	public delegate void deleg(string par);
	private deleg func;
	private Dictionary<string, deleg> functions = new Dictionary<string, deleg>();

	void Start () {
		instance = this;
		deleg nFunc = CreateObj;
		AddCall("new", nFunc);

	}
	
	public void AddCall(string name, deleg method){
		name.ToLower();

		if (!functions.ContainsKey(name)){
			functions.Add(name, method);
		}
    }

	public void CallFunction(string name, string par){
		name.ToLower();
		
		if (functions.ContainsKey(name)){
			functions[name].Invoke(par);
		}
	}

	public void ParceInstruction(string instruction){
		instruction.ToLower();
		string funcName = "";
		string par = "";
		
		bool parame = false;
		bool done = false;
		int i = 0;
		while (!done) 
		{ 
    		switch (instruction[i]) 
    		{ 
    		    case '(':
					parame = true;
					break;
				case ')':
					done = true;
					break;
				case ' ':
					done = true;
					break;
				default:
					if (!parame)
						funcName += instruction[i];
					else
						par += instruction[i];
					break;
    		} 
			i++;
		}

		CallFunction(funcName, par);
	}

	private void CreateObj(string objName){
		GameObject instance = Instantiate(Resources.Load(objName, typeof(GameObject))) as GameObject;
	}
}
