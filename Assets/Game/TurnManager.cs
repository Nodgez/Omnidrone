﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
	private static TurnManager instance;
	public static TurnManager Instance
	{
		get { return instance; }
	}
	public int TurnNumber { get; private set; }
	public ArmyController ActiveArmy { get; private set; }

	public ArmyController[] armies;

	public void Start()
	{
		instance = this;
		ActiveArmy = armies[TurnNumber % armies.Length];
		ActiveArmy.StartArmy();
		MessagePopup.Instance.Show(ActiveArmy.tag + " turn");

		foreach (var army in armies)
			army.onArmyLost.AddListener(() => {
				MessagePopup.Instance.Show(army.name + " Lost", 
											new System.Tuple<string, UnityAction>("Play Again", PlayAgain),
											new System.Tuple<string, UnityAction>("Quit", QuitGame)); 
				
				});
	}

	public void EndTurn()
	{
		ActiveArmy.FinalizeArmy();
		TurnNumber++;
		ActiveArmy = armies[TurnNumber % armies.Length];
		if (ActiveArmy.HasLost)
			return;
		ActiveArmy.StartArmy();
		MessagePopup.Instance.Show(ActiveArmy.tag + " turn");
	}

	private void PlayAgain()
	{
		SceneManager.LoadScene(0);
	}

	private void QuitGame()
	{
		Application.Quit();
	}
}
