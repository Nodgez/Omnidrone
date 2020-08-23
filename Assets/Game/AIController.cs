using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : ArmyController
{
	public override void FinalizeArmy()
	{
		
	}

	public override void StartArmy()
	{
		StartCoroutine(CO_UpdateArmy());
	}

	IEnumerator CO_UpdateArmy()
	{
		yield return new WaitForSeconds(5f);
		TurnManager.Instance.EndTurn();
	}
}
