using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour {
	static public Console instance;
	public delegate void deleg(string par);
	
	private Dictionary<string, deleg> functions = new Dictionary<string, deleg>();

	void Awake () {
		instance = this;
	}
	
	public void AddCall(string name, deleg method){
		name.ToLower();

		if (!functions.ContainsKey(name)){
			functions.Add(name, method);
		}
    }

	public string CallFunction(string instruction){
		instruction.ToLower();
		string outLog;
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

		if (functions.ContainsKey(funcName)){
			outLog = "Comando Ejecutado: " + funcName + " \nParametro: " + par + "\n\n";
			functions[funcName](par);
		}
		else
			outLog = "Comando " + funcName + " no existe\n\n";

		return outLog;
	}
}
