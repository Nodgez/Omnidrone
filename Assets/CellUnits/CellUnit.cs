using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public abstract class CellUnit : MonoBehaviour, IInteractable
{
	[SerializeField]
	private CellUnitStats unitStats = null;

	private Animator animator;

	public Cell currentCell;
	public Indicator hitIndicator;

	[HideInInspector]
	public OnDeath onDeath = new OnDeath();
	public OnHealthAltered onHealthAltered= new OnHealthAltered();

	public int CurrentMovementRange { get; private set; }
	public int CurrentHealth { get; private set; }
	public int MaxHealth { get { return unitStats.maxHealth; } }
	public int AttackRange { get { return unitStats.attackRange; } }
	public int VisionRange{ get { return unitStats.visionRange; } }
	public int AttackDamage { get { return unitStats.damage; } }

	public int Attacks { get; private set;}

	private void Awake()
	{
		animator = GetComponent<Animator>();
		RefreshStats(true);
	}
	public void MoveAlongPath(Stack<Cell> path, Action onMoveComplete = null)
	{
		if (CurrentMovementRange <= 0)
			return;
		StartCoroutine(CO_MoveAlongPath(path, onMoveComplete));
	}
	
	public void MoveAlongPath(Stack<Cell> path, out Coroutine moveRoutine, Action onMoveComplete = null)
	{
		if (CurrentMovementRange <= 0)
			moveRoutine = null;
		else
			moveRoutine = StartCoroutine(CO_MoveAlongPath(path, onMoveComplete));
	}

	private  IEnumerator CO_MoveAlongPath(Stack<Cell> path, Action onMoveComplete)
	{
		while (path.Count > 0)
		{
			var target = path.Pop();
			target.cellUnit = this;

			float t = 0;
			var startPosition = transform.position;
			var startRotation = transform.rotation;
			
			while (t < 1)
			{
				t += Mathf.Clamp01(t + Time.deltaTime);
				transform.position = Vector3.Lerp(startPosition, target.transform.position, t);
				yield return null;
			}
			CurrentMovementRange--;

			currentCell.cellUnit = null;
			currentCell = target;
		}

		onMoveComplete?.Invoke();
	}

	public void RefreshStats(bool withHealth = false)
	{
		Attacks = 1;
		CurrentMovementRange = unitStats.moveRange;

		if (withHealth)
			CurrentHealth = MaxHealth;
	}

	public void AlterHealth(int changeInHealth)
	{
		CurrentHealth += changeInHealth;

		if (onHealthAltered != null)
			onHealthAltered.Invoke(this);

		var negative = changeInHealth < 0;
		if (negative)
			hitIndicator.Indicate();
	
		if (CurrentHealth <= 0)
		{
			currentCell.cellUnit = null;
			if (onDeath != null)
				onDeath.Invoke(this);
			Destroy(this.gameObject);
		}
	}
	public void Attack(CellUnit target)
	{
		if (Attacks < 1 || Cell.Distance(target.currentCell.cubePoint, currentCell.cubePoint) > AttackRange)
			return;
		Attacks--;
		target.AlterHealth(-AttackDamage);
		//animate
	}

	public void Interact()
	{
		currentCell.Interact();
	}


#if UNITY_EDITOR
	public void SetOnCell(Cell cell)
	{
		if (cell.cellUnit != null)
			DestroyImmediate(currentCell.cellUnit);

		currentCell = cell;
		currentCell.cellUnit = this;
	}
#endif
}

[Serializable]
public class OnDeath : UnityEvent<CellUnit> { }
public class OnHealthAltered : UnityEvent<CellUnit> { }