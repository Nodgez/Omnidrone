using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyController : MonoBehaviour
{
	private CellUnit selectedUnit;
	private CellUnit selectedTarget;

	public int SelectionLimit { get; set; }

	private List<Cell> actionableCellRange = new List<Cell>();

	private void Awake()
	{
		SelectionLimit = 1;
	}

	public void SelectUnit(CellUnit unit)
	{
		ClearActionableCellRange();

		//selected ally unity
		if (unit.tag == tag)
		{
			selectedUnit = unit;
			actionableCellRange = Battlefield.Instance.Search(selectedUnit.currentCell, selectedUnit.unitStats.moveRange);
		}
		else
			selectedTarget = unit;

		SetActionableCellRange();
	}

	private void SetActionableCellRange()
	{
		var moveMaterial = new MaterialPropertyBlock();
		moveMaterial.SetColor("_Color", Color.blue);
		foreach (var cell in actionableCellRange)
			cell.meshRenderer.SetPropertyBlock(moveMaterial);
	}
	
	private void ClearActionableCellRange()
	{
		foreach (var cell in actionableCellRange)
			cell.meshRenderer.SetPropertyBlock(null);
	}

	public void MoveSelectedUnit(Cell target)
	{
		if (selectedUnit == null)
			return;
		print(string.Format("Moving Unit From: {0} , To: {1} ", selectedUnit.currentCell.ToString(), target.ToString()));
		var movePath = Battlefield.Instance.FindPath(selectedUnit.currentCell, target);
		selectedUnit.MoveAlongPath(movePath);

		ClearActionableCellRange();
	}
}
