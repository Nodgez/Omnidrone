using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyController : MonoBehaviour
{
	private CellUnit selectedUnit;
	private List<CellUnit> selectedTargets;

	public int SelectionLimit { get; set; }

	private void Awake()
	{
		SelectionLimit = 1;
	}

	public void SelectUnit(CellUnit unit)
	{
		//selected ally unity
		if (unit.tag == tag)
		{
			print("Selecting Unit");
			selectedUnit = unit;
		}
	}

	public void MoveSelectedUnit(Cell target)
	{
		if (selectedUnit == null)
			return;
		print(string.Format("Moving Unit From: {0} , To: {1} ", selectedUnit.currentCell.ToString(), target.ToString()));
		var movePath = Battlefield.Instance.FindPath(selectedUnit.currentCell, target);
		selectedUnit.MoveAlongPath(movePath);
	}
}
