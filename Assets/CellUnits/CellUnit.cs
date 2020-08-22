using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellUnit : MonoBehaviour
{
	public CellUnitStats unitStats;
	public Cell currentCell;

	public int CurrentMoveCount { get; private set; }
	public int CurrentHealth { get; private set; }

	private void Awake()
	{
		RefreshStats();
	}
	public void MoveAlongPath(Stack<Cell> path)
	{
		if (CurrentMoveCount <= 0)
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
			CurrentMoveCount--;

			currentCell.cellUnit = null;
			currentCell = target;
		}
	}

	public void RefreshStats()
	{
		CurrentMoveCount = unitStats.moveRange;
	}

	public void AlterHealth(int changeInHealth)
	{
		CurrentHealth += changeInHealth;

		if (CurrentHealth == 0)
		{
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
#endif
}
