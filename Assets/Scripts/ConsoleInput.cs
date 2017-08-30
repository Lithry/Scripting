using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleInput : MonoBehaviour {
	public InputField inField;

	void Start () {
		Console.deleg nFunc = CreateObj;
		Console.instance.AddCall("new", nFunc);

	}

	// Update is called once per frame
	void Update () {
		if(inField.isFocused && inField.text != "" && Input.GetKey(KeyCode.Return)) {
         	string log = Console.instance.CallFunction(inField.text);
			ConsoleLog.instance.WriteText(log);
         	inField.text = "";
     	}
	}

	private void CreateObj(string objName){
		GameObject instance = Instantiate(Resources.Load(objName, typeof(GameObject))) as GameObject;
	}
}
