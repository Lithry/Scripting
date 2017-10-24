using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameError : ErrorManager {
	
	override public void ParcerLogError(string error){
		Debug.Log("[Parcer] - " + error);
	}

	override public void ByteCodeLogError(string error){
		Debug.Log("[ByteCode] - " + error);
	}

}
