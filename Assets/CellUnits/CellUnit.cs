using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public abstract class CellUnit : MonoBehaviour
{
	[SerializeField]
	private CellUnitStats unitStats = null;
	public Cell currentCell;
	public OnDeath onDeath = new OnDeath();

	private MeshRenderer meshRenderer;

	public int CurrentMovementRange { get; private set; }
	public int CurrentHealth { get; private set; }
	public int AttackRange { get { return unitStats.attackRange; } }
	public int AttackDamage { get { return unitStats.damage; } }

	public int Attacks { get; private set;}

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		RefreshStats();
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

		onMoveComplete?.Invoke();
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
		}
	}
	public void Attack(CellUnit target)
	{
		if (Attacks < 1)
			return;
		Attacks--;
		target.AlterHealth(-AttackDamage);
		//animate
	}

	public void Highlight()
	{
		var highlightProperty = new MaterialPropertyBlock();
		highlightProperty.SetFloat("_FirstOutlineWidth", 0.25f);

		meshRenderer.SetPropertyBlock(highlightProperty);
	}
	
	public void UnHighlight()
	{
		meshRenderer.SetPropertyBlock(null);
	}



#if UNITY_EDITOR
	public void SetOnCell(Cell cell)
	{
		if (currentCell != null)
			currentCell.cellUnit = null; 

		currentCell = cell;
		currentCell.cellUnit = this;
	}
#endif
}

[Serializable]
public class OnDeath : UnityEvent<CellUnit> { }