using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
			yield return StartCoroutine(SearchForAndAttack());

			var enemiesInVision = Battlefield.Instance.GetCellsInRange(selectedUnit.currentCell, selectedUnit.VisionRange, (cell) => { return cell.Occupied && !cell.IsAllyCell(tag); });
			var movementCells = Battlefield.Instance.Search(selectedUnit.currentCell, selectedUnit.CurrentMovementRange);
			if (movementCells.Count > 0)
			{
				Cell closeEnemy = null;
				if (enemiesInVision.Count > 0)
					closeEnemy = enemiesInVision.GetRandomElement();

				var selectedTarget = closeEnemy == null ? movementCells.GetRandomElement() : movementCells.OrderBy((cell) => Cell.Distance(cell.cubePoint, closeEnemy.cubePoint)).First();
				MoveSelectedUnit(selectedTarget);
				if (moveRoutine != null)
					yield return moveRoutine;

				yield return StartCoroutine(SearchForAndAttack());

			}
		}
		yield return new WaitForSeconds(1f);
		GameController.Instance.EndTurn();
	}

	private IEnumerator SearchForAndAttack()
	{
		var enemyCells = selectedUnit.Attacks < 1 ? new List<Cell>() :
							Battlefield.Instance.GetCellsInRange(selectedUnit.currentCell, selectedUnit.AttackRange, (cell) => { return cell.Occupied && !cell.IsAllyCell(tag); });

		if (enemyCells.Count > 0)
		{
			SelectUnit(enemyCells[0].cellUnit);
			yield return new WaitForSeconds(1f);
		}
	}
}
