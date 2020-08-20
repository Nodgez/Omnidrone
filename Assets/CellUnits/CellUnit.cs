using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellUnit : MonoBehaviour, IInteractable
{
	private Cell currentCell;

	public virtual void Interact()
	{
	}


#if UNITY_EDITOR
	public void SetOnCell(Cell cell)
	{
		if (currentCell != null)
			currentCell.CellUnit = null; 

		currentCell = cell;
		currentCell.CellUnit = this;
	}
#endif
}
