using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CompileTest : MonoBehaviour 
{
	public Button compileButton;
	public InputField inputField;

	Compiler compiler = new Compiler();	

	// Use this for initialization
	void Awake () 
	{
		compileButton.onClick.AddListener(OnClick);	
	}

	void OnClick()
	{
		if (compiler.Compile(inputField.text))
		{
			// TODO: Convert to bytecode and create Script
		}
	}
}
