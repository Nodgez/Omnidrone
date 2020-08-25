using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWarrior : CellUnit
{
	public Indicator selectionIndicator;

	public void Highlight()
	{
		selectionIndicator.Indicate();
	}

	public void UnHighlight()
	{
		selectionIndicator.Conceal();
	}
}
