using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleView : MonoBehaviour
{
	public Text ConsoleTxt;
	public InputField InputConsole;
	private Dictionary<string, string> vars = new Dictionary<string, string>();

	// Use this for initialization
	void Start () {
		InputConsole.onEndEdit.AddListener(OnEndEdit);
		DebugConsole.Instance.RegisterCommand("Log", LogCommand);
		DebugConsole.Instance.RegisterCommand("CreateObject", CreateObjectCommand);
		DebugConsole.Instance.RegisterCommand("SaveVar", SaveVarCommand);
	}

	void OnEndEdit(string val)
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			Debug.Log(val);
			if (!string.IsNullOrEmpty(val))
			{
				ExecuteCommand(val);
				InputConsole.text = "";
			}
		}
	}

	void ExecuteCommand(string str)
	{
		string log = DebugConsole.Instance.ExecuteCommand(str);
		Log(log);
	}

	void Log(string str)
	{
		ConsoleTxt.text += "\n" + str;
	}

	string LogCommand(params string[] args)
	{
		string str = "";

		if (args != null)
		{
			for (int i = 0; i < args.Length; i++)
			{
				str += args[i] + "\n";
			}

			return str;
		}

		return "No arguments...";
	}

	string CreateObjectCommand(params string[] args)
	{
		if (args != null && args.Length > 0)
		{
			GameObject prefab = Resources.Load<GameObject>(args[0]);
			if (prefab)
			{
				GameObject.Instantiate(prefab);
				return "OK";
			}
			else
				return "Error loading \"" + args[0] + "\" prefab.";
		}


		return "No arguments...";
	}

	string SaveVarCommand(params string[] args){
		if (args != null && args.Length == 2){
			if (args[1].Length == 1){					//
				if (char.IsLetter(args[1][0])){			// if arg is a var
					if (vars.ContainsKey(args[1])){		//
						if (vars.ContainsKey(args[0])){
							vars[args[0]] = vars[args[1]];
							return "Var " + args[0] + " has now the value of var " + args[1] + " (\"" + vars[args[1]] + "\")";
						}
						else{
							vars.Add(args[0], args[1]);
							return "New Var " + args[0] + " has the value of var " + args[1] + " (\"" + vars[args[1]] + "\")";
						}
					}
				}
			}
			
			if (vars.ContainsKey(args[0])){
				vars[args[0]] = args[1];
				return "Var " + args[0] + " has now the value of " + "\"" + args[1] + "\"";
			}
			else{
				vars.Add(args[0], args[1]);
				return "New Var " + args[0] + " has the value of " + "\"" + args[1] + "\"";
			}
		}

		return "Error in arguments...";
	}
}
