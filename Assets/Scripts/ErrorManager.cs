﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorManager {

	virtual public void ParserLogError(string error){}
	virtual public void ByteCodeLogError(string error){}
	virtual public void RunTimeLogError(string error){}
}
