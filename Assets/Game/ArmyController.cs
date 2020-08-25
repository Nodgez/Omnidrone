using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class ArmyController : MonoBehaviour
{
	protected CellUnit selectedUnit;
	protected List<Cell> actionableCells = new List<Cell>();

	[SerializeField]
	protected List<CellUnit> army = new List<CellUnit>();

	public bool HasLost{ get{ return army.Count < 1; } }
	public abstract void StartArmy();
	public abstract void FinalizeArmy();
	public abstract void SelectUnit(CellUnit unit);
	public abstract void MoveSelectedUnit(Cell target);

	[HideInInspector]
	public UnityEvent onArmyLost = new UnityEvent();

	protected virtual void Awake()
	{
		army.ForEach(unit => InitializeUnit(unit));
	}

	public void RemoveUnitFromArmy(CellUnit unitToRemove)
	{
		army.Remove(unitToRemove);

		if (army.Count < 1)
			onArmyLost?.Invoke();
	}

	protected void RefreshArmy()
	{
		foreach (var unit in army)
		{
			unit.RefreshStats();
		}
	}

	private void InitializeUnit(CellUnit unit)
	{
		unit.onDeath.AddListener(RemoveUnitFromArmy);
		DiageticUi.Instance.AddUnitHPBar(unit);
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
