using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleLog : MonoBehaviour {
	static public ConsoleLog instance;
	public Text log;	
	// Use this for initialization
	void Start () {
		instance = this;
		log.text = "";
	}
	
	// Update is called once per frame
	public void WriteText(string d){
		log.text += d;
	}
}
