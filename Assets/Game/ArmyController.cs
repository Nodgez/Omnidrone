using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class ArmyController : MonoBehaviour
{
	protected CellUnit selectedUnit;
	protected List<Cell> actionableCells = new List<Cell>();

	[SerializeField]
	protected List<CellUnit> army = new List<CellUnit>();

	public int SelectionLimit { get; set; }
	public bool HasLost{ get{ return army.Count > 0; } }
	public abstract void StartArmy();
	public abstract void FinalizeArmy();
	public abstract void SelectUnit(CellUnit unit);

	protected virtual void Awake()
	{
		SelectionLimit = 1;
		army.ForEach(unit => unit.onDeath.AddListener(RemoveUnitFromArmy));
	}

	public void RemoveUnitFromArmy(CellUnit unitToRemove)
	{
		army.Remove(unitToRemove);
	}

	protected void RefreshArmy()
	{
		foreach (var unit in army)
		{
			unit.RefreshStats();
		}
	}

	protected void SetActionableCellMarkers()
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
	
	protected void ClearActionableCellRange()
	{
		foreach (var cell in actionableCells)
			cell.meshRenderer.SetPropertyBlock(null);

		actionableCells.Clear();
	}

	public void MoveSelectedUnit(Cell target)
	{
		if (selectedUnit == null)
			return;
		var movePath = Battlefield.Instance.FindPath(selectedUnit.currentCell, target, selectedUnit.CurrentMovementRange);
		selectedUnit.MoveAlongPath(movePath);

		ClearActionableCellRange();
	}

	public void AttackUnitOnCell(CellUnit target)
	{
		if (selectedUnit != null && selectedUnit.Attacks > 0)
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
