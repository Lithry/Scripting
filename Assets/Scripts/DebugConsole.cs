using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour 
{
	public delegate string CommandDlg(params string[] args);
	private static DebugConsole instance = null;

	private Dictionary<string, CommandDlg> commands = new Dictionary<string, CommandDlg>();

	Parser parser = new Parser();

	public static DebugConsole Instance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType<DebugConsole>();
			
			return instance;
		}
	}

	// Use this for initialization
	void Awake () {
		instance = this;
	}

	public void RegisterCommand(string cmdStr, CommandDlg cmd)
	{
		commands[cmdStr.ToLower()] = cmd;
	}

	public string ExecuteCommand(string str)
	{
		Command cmd = null;
		
		if (parser.Parse(str, out cmd))
		{
			string cmdName = cmd.CommandName.ToLower();

			if (commands.ContainsKey(cmdName))
			{
				string log = "Executing command: " + cmdName + "\n";
				string[] args = cmd.Args.ToArray();

				log += commands[cmdName](args);
				
				return log;
			}
			else
			{
				return "ERROR: Command \"" + cmd + "\" not found.";
			}
		}
		else
		{
			return "Syntax error!";
		}
	}
}
