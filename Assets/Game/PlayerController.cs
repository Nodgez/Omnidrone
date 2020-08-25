using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : ArmyController
{
	public CanvasGroup playerUI;
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
		selectedUnit = army[0];
		SelectUnit(selectedUnit);
		playerUI.On();
	}

	public override void FinalizeArmy()
	{
		input.enabled = false;
		ClearActionableCellRange();
		(selectedUnit as CubeWarrior).UnHighlight();
		playerUI.Off();
	}

	public override void SelectUnit(CellUnit unit)
	{
		//selected ally unity
		if (unit.tag == tag)
		{
			(selectedUnit as CubeWarrior).UnHighlight();

			selectedUnit = unit;
			(selectedUnit as CubeWarrior).Highlight();

			FindAndShowActionableCells();
		}
		else
		{
			selectedUnit.Attack(unit);
			FindAndShowActionableCells();

		}

	}

	public override void MoveSelectedUnit(Cell target)
	{
		var movePath = Battlefield.Instance.FindPath(selectedUnit.currentCell, target, selectedUnit.CurrentMovementRange);
		if (movePath.Count > 0)
		{
			selectedUnit.MoveAlongPath(movePath, FindAndShowActionableCells);
			ClearActionableCellRange();
		}
	}

	private void FindAndShowActionableCells()
	{
		ClearActionableCellRange();
		var movementCells = Battlefield.Instance.Search(selectedUnit.currentCell, selectedUnit.CurrentMovementRange);// (cell) => { return !cell.Occupied; });
		var enemyCells = selectedUnit.Attacks < 1 ? new List<Cell>() :
			Battlefield.Instance.GetTilesInRange(selectedUnit.currentCell, selectedUnit.AttackRange, (cell) => { return cell.Occupied && !cell.IsAllyCell(tag); });

		actionableCells = new List<Cell>();
		actionableCells.AddRange(enemyCells);
		actionableCells.AddRange(movementCells);
		SetActionableCellMarkers();
	}
}
