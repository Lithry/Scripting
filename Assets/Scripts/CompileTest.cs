using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CompileTest : MonoBehaviour 
{
	public Button compileButton;
	public InputField inputField;

	ErrorManager errorHandler;
	Compiler compiler;
	Script script;

	// Use this for initialization
	void Awake () 
	{
		errorHandler = new MyGameError();
		compiler = new Compiler(errorHandler);
		compileButton.onClick.AddListener(OnClick);	
	}

	void OnClick()
	{
		if (compiler.Compile(inputField.text))
		{
			MemoryStream ms = ByteCode.SaveToMemory(compiler.GetTables());
			
			ScriptContext context;

			if (ByteCode.Load(ms, out context, errorHandler))
			{
				script = new Script(context, errorHandler);
				script.Start();
			}
		}
	}

	void Update()
	{
		if (script != null)
			script.RunStep();
	}
}
