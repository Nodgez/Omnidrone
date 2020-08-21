using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cell Unit Stats", menuName = "RPG Arena/Create Cell Unit Stats")]
public class CellUnitStats : ScriptableObject
{
	[Range(50,100)]
	public int maxHealth = 100;
	[Range(1, 5)]
	public int moveRange = 3;
	[Range(1,5)]
	public int attackRange = 2;
	[Range(5, 50)]
	public int damage = 10;
}
