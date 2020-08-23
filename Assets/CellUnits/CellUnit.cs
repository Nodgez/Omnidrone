using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CellUnit : MonoBehaviour
{
	[SerializeField]
	private CellUnitStats unitStats = null;
	public Cell currentCell;

	public OnDeath onDeath = new OnDeath();

	public int CurrentMovementRange { get; private set; }
	public int CurrentHealth { get; private set; }
	public int AttackRange { get { return unitStats.attackRange; } }
	public int AttackDamage { get { return unitStats.damage; } }

	public int Attacks = 1;

	private void Awake()
	{
		RefreshStats();
	}
	public void MoveAlongPath(Stack<Cell> path)
	{
		if (CurrentMovementRange <= 0)
			return;
		StartCoroutine(CO_MoveAlongPath(path));
	}

	private  IEnumerator CO_MoveAlongPath(Stack<Cell> path)
	{
		while (path.Count > 0)
		{
			var target = path.Pop();
			target.cellUnit = this;

			float t = 0;
			Vector3 start = transform.position;
			while (t < 1)
			{
				t += Mathf.Clamp01(t + Time.deltaTime);
				transform.position = Vector3.Lerp(start, target.transform.position, t);
				yield return null;
			}
			CurrentMovementRange--;

			currentCell.cellUnit = null;
			currentCell = target;
		}
	}

	public void RefreshStats()
	{
		Attacks = 1;
		CurrentMovementRange = unitStats.moveRange;
		CurrentHealth = unitStats.maxHealth;
	}

	public void AlterHealth(int changeInHealth)
	{
		CurrentHealth += changeInHealth;

		if (CurrentHealth <= 0)
		{
			onDeath?.Invoke(this);
			Destroy(this.gameObject);
			print("Unit Dead");
			//remove from the army and teh battlefield
		}
	}



#if UNITY_EDITOR
	public void SetOnCell(Cell cell)
	{
		if (currentCell != null)
			currentCell.cellUnit = null; 

		currentCell = cell;
		currentCell.cellUnit = this;
	}

	public void Attack()
	{
		Attacks--;
	}
#endif
}

[Serializable]
public class OnDeath : UnityEvent<CellUnit> { }