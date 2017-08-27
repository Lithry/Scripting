using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MoonSharp.Interpreter;

public class MoveObject : MonoBehaviour {
	Script script = new Script();

	void Start () {
		string scriptCode = System.IO.File.ReadAllText("Assets/Resources/LuaTest.lua");
		
		UserData.RegisterType<MoveObject>();
		
		DynValue obj = UserData.Create(this);
		
		script.Globals.Set("obj", obj);

		script.DoString(scriptCode);
	}
	
	public void Move(float x) {
		Vector3 pos = this.transform.position;
		pos.x += x;
		this.transform.position = pos;
	}

	public void RotateRight(float y){
		Vector3 goal = this.transform.right - this.transform.position;
		Vector3 newDir = Vector3.RotateTowards(this.transform.forward, goal, y, 0.0f);
		this.transform.rotation = Quaternion.LookRotation(newDir);
	}

	public void RotateLeft(float y){
		Vector3 goal = -this.transform.right - this.transform.position;
		Vector3 newDir = Vector3.RotateTowards(this.transform.forward, goal, y, 0.0f);
		this.transform.rotation = Quaternion.LookRotation(newDir);
	}

	// Update is called once per frame
	void Update () {
		script.Call(script.Globals["update"], Time.deltaTime);						
	}
}
