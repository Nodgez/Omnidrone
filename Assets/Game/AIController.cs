using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : ArmyController
{
	public override void FinalizeArmy()
	{
		
	}

	public override void SelectUnit(CellUnit unit)
	{
		selectedUnit = unit;
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

			var movementCells = Battlefield.Instance.GetTilesInRange(selectedUnit.currentCell, selectedUnit.CurrentMovementRange, (cell) => { return !cell.Occupied; });

			if (enemyCells.Count > 0)
				AttackUnitOnCell(enemyCells[0].cellUnit);
				
			if (movementCells.Count > 0)
				MoveSelectedUnit(movementCells.GetRandomElement());

			yield return new WaitForSeconds(2f);
		}
		yield return new WaitForSeconds(2f);
		TurnManager.Instance.EndTurn();
	}
}
