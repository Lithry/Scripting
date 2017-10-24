using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ByteCodeManager : MonoBehaviour {

	private struct FileFormat{
		public const string magic = "STP5";
		public const int magicLength = 4;
		public const int mayorVer = 1;
		public const int minorVer = 0;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SaveFile(){
		BinaryWriter writer = new BinaryWriter(File.Open("Assets/Resources/file.txt", FileMode.Create));

		writer.Write(FileFormat.magic);
		writer.Write(FileFormat.mayorVer);
		writer.Write(FileFormat.minorVer);
	}

	public void LoadFile(){
		BinaryReader reader = new BinaryReader(File.Open("Assets/Resources/file.txt", FileMode.Open));

		// Read Magic
		reader.ReadString();
		
		// Read Upper Ver
		reader.ReadInt32();

		// Read LowerVer
		reader.ReadInt32();
	}
}
