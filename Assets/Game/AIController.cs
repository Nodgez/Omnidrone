using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : ArmyController
{

	private Coroutine moveRoutine = null;

	public override void FinalizeArmy()
	{
		
	}

	public override void SelectUnit(CellUnit unit)
	{
		if (unit.CompareTag(tag))
			selectedUnit = unit;
		else
			selectedUnit.Attack(unit);
	}

	public override void MoveSelectedUnit(Cell target)
	{
		var movePath = Battlefield.Instance.FindPath(selectedUnit.currentCell, target, selectedUnit.CurrentMovementRange);
		selectedUnit.MoveAlongPath(movePath, out moveRoutine);
		ClearActionableCellRange();
	}

	public override void StartArmy()
	{
		StartCoroutine(CO_UpdateArmy());
		RefreshArmy();

	}

	IEnumerator CO_UpdateArmy()
	{
		foreach (var unit in army)
		{
			SelectUnit(unit);
			var enemyCells = selectedUnit.Attacks < 1 ? new List<Cell>() :
							Battlefield.Instance.GetTilesInRange(selectedUnit.currentCell, selectedUnit.AttackRange, (cell) => { return cell.Occupied && !cell.IsAllyCell(tag); });

			var movementCells = Battlefield.Instance.Search(selectedUnit.currentCell, selectedUnit.CurrentMovementRange);// (cell) => { return !cell.Occupied; });

			if (enemyCells.Count > 0)
			{
				SelectUnit(enemyCells[0].cellUnit);
				yield return new WaitForSeconds(1f);
			}

			if (movementCells.Count > 0)
			{
				MoveSelectedUnit(movementCells.GetRandomElement());
				if (moveRoutine != null)
					yield return moveRoutine;
			}
		}
		yield return new WaitForSeconds(2f);
		GameController.Instance.EndTurn();
	}
}
