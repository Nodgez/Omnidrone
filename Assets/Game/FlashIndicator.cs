using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashIndicator : Indicator
{
	public Color indicationRGB = new Color(1, 1, 1, 1);

	public override void Conceal()
	{
	}

	public override void Indicate()
	{
		gameObject.ColorFrom(Color.red, 0.25f, 0f);
	}
}
