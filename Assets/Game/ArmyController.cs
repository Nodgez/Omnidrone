using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyController : MonoBehaviour
{
	private CellUnit selectedUnit;
	private CellUnit selectedTarget;

	[SerializeField]
	private List<CellUnit> army = new List<CellUnit>();

	public int SelectionLimit { get; set; }

	private List<Cell> actionableCells = new List<Cell>();

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
			actionableCells = Battlefield.Instance.GetTilesInRange(selectedUnit.currentCell, selectedUnit.CurrentMoveCount, (x) => { return !x.Occupied; });
		}
		else
			AttackSelectedUnit(unit);

		SetActionableCellMarkers();
	}

	public void RefreshArmy()
	{
		foreach (var unit in army)
		{
			unit.RefreshStats();
		}
	}

	private void SetActionableCellMarkers()
	{
		var moveMaterial = new MaterialPropertyBlock();
		moveMaterial.SetColor("_Color", Color.blue);
		
		var attackMaterial = new MaterialPropertyBlock();
		attackMaterial.SetColor("_Color", Color.red);
		foreach (var cell in actionableCells)
		{
			if (cell.Occupied)
			{
				if (!cell.cellUnit.CompareTag(tag))
					cell.meshRenderer.SetPropertyBlock(attackMaterial);
			}

			else
				cell.meshRenderer.SetPropertyBlock(moveMaterial);
		}
	}
	
	private void ClearActionableCellRange()
	{
		foreach (var cell in actionableCells)
			cell.meshRenderer.SetPropertyBlock(null);

		actionableCells.Clear();
	}

	public void MoveSelectedUnit(Cell target)
	{
		if (selectedUnit == null)
			return;
		print(string.Format("Moving Unit From: {0} , To: {1} ", selectedUnit.currentCell.ToString(), target.ToString()));
		var movePath = Battlefield.Instance.FindPath(selectedUnit.currentCell, target, selectedUnit.unitStats.moveRange);
		selectedUnit.MoveAlongPath(movePath);

		ClearActionableCellRange();
	}

	public void AttackSelectedUnit(CellUnit target)
	{
		if (selectedUnit != null)
			target.AlterHealth(selectedUnit.unitStats.damage);
	}

#if UNITY_EDITOR
	public void AppendToArmy(CellUnit cellUnit)
	{
		army.Add(cellUnit);
	}
	#endif
}
