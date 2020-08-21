using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	private static TurnManager instance;
	public static TurnManager Instance
	{
		get { return instance; }
	}
	public int TurnNumber { get; private set; }
	public ArmyController[] armies;
	public ArmyController ActiveArmy { get; private set; }

	public void Start()
	{
		instance = this;
		ActiveArmy = armies[TurnNumber % armies.Length];
	}

	public IEnumerator RunTurn()
	{
		yield return null;

		TurnNumber++;
	}

	public void EndTurn()
	{
		TurnNumber++;
	}
}
