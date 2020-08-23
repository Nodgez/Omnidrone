using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : ArmyController
{
	private PlayerInput input;
	protected override void Awake()
	{
		input = GetComponent<PlayerInput>();
		base.Awake();
	}
	public override void StartArmy()
	{
		input.enabled = true;
		RefreshArmy();
	}

	public override void FinalizeArmy()
	{
		input.enabled = false;
		ClearActionableCellRange();
	}

	public override void SelectUnit(CellUnit unit)
	{
		ClearActionableCellRange();

		//selected ally unity
		if (unit.tag == tag)
		{
			selectedUnit = unit;
			var movementCells = Battlefield.Instance.GetTilesInRange(selectedUnit.currentCell, selectedUnit.CurrentMovementRange, (cell) => { return !cell.Occupied; });
			var enemyCells = selectedUnit.Attacks < 1 ? new List<Cell>() :
				Battlefield.Instance.GetTilesInRange(selectedUnit.currentCell, selectedUnit.AttackRange, (cell) => { return cell.Occupied && !cell.IsAllyCell(tag); });

			actionableCells = new List<Cell>();
			actionableCells.AddRange(enemyCells);
			actionableCells.AddRange(movementCells);
		}
		else
			AttackUnitOnCell(unit);

		SetActionableCellMarkers();
	}
}
