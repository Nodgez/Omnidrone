using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class ArmyController : MonoBehaviour
{
	private CellUnit selectedUnit;
	private CellUnit selectedTarget;
	private List<Cell> actionableCells = new List<Cell>();

	[SerializeField]
	private List<CellUnit> army = new List<CellUnit>();

	public int SelectionLimit { get; set; }
	public bool HasLost{ get{ return army.Count > 0; } }
	public abstract void StartArmy();
	public abstract void FinalizeArmy();
	protected virtual void Awake()
	{
		SelectionLimit = 1;
		army.ForEach(unit => unit.onDeath.AddListener(RemoveUnitFromArmy));
	}
	

	public void SelectUnit(CellUnit unit)
	{
		ClearActionableCellRange();

		//selected ally unity
		if (unit.tag == tag)
		{
			selectedUnit = unit;
			var movementCells = Battlefield.Instance.GetTilesInRange(selectedUnit.currentCell, selectedUnit.CurrentMovementRange, (x) => { return !x.Occupied; });
			var enemyCells = Battlefield.Instance.GetTilesInRange(selectedUnit.currentCell, selectedUnit.AttackRange, (x) => { return x.Occupied && !x.cellUnit.CompareTag(tag); });

			actionableCells = new List<Cell>();
			actionableCells.AddRange(enemyCells);
			actionableCells.AddRange(movementCells);
		}
		else
			AttackSelectedUnit(unit);

		SetActionableCellMarkers();
	}

	public void RemoveUnitFromArmy(CellUnit unitToRemove)
	{
		army.Remove(unitToRemove);
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
		var movePath = Battlefield.Instance.FindPath(selectedUnit.currentCell, target, selectedUnit.CurrentMovementRange);
		selectedUnit.MoveAlongPath(movePath);

		ClearActionableCellRange();
	}

	public void AttackSelectedUnit(CellUnit target)
	{
		if (selectedUnit != null)
		{
			selectedUnit.Attack();
			target.AlterHealth(-selectedUnit.AttackDamage);
		}
	}

#if UNITY_EDITOR
	public void AppendToArmy(CellUnit cellUnit)
	{
		army.Add(cellUnit);
	}
	
	public void ClearEmptyUnits()
	{
		for (var i = army.Count - 1;i > -1; i--)
		{
			if (army[i] == null)
				army.RemoveAt(i);
		}
	}
	#endif
}
