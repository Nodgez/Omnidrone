using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : ArmyController
{
	private PlayerInput input;
	protected override void Awake()
	{
		input = GetComponent<PlayerInput>();
		base.Awake();
	}
	public override void StartArmy()
	{
		input.enabled = true;
	}

	public override void FinalizeArmy()
	{
		input.enabled = false;
	}
}
