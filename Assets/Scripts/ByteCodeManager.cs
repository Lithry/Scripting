using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ByteCodeManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SaveFile(){
		BinaryWriter writer = new BinaryWriter(File.Open("Assets/Resources/file.txt", FileMode.Create));

		writer.Write(FileFormat.magic);
		writer.Write(FileFormat.upperVer);
		writer.Write(FileFormat.lowerVer);
	}

	public void LoadFile(){
		BinaryReader reader = new BinaryReader(File.Open("Assets/Resources/file.txt", FileMode.Open));

		// Read Magic
		reader.ReadChars(FileFormat.magicLength);
		
		// Read Upper Ver
		reader.ReadInt32();

		// Read LowerVer
		reader.ReadInt32();
	}
}
