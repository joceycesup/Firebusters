using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBAxe : FBTool {
	
	public override void Enable () {
		tag = "Axe";
	}

	public override void Disable () {
		tag = "Untagged";
	}
}
