using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleInput : MonoBehaviour {
	public InputField inField;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(inField.isFocused && inField.text != "" && Input.GetKey(KeyCode.Return)) {
         	Console.instance.ParceInstruction(inField.text);
         	inField.text = "";
     	}
	}
}
