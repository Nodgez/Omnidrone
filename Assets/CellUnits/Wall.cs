using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : CellUnit
{
	public override void Interact()
	{
		print("I am wall");
	}
}
